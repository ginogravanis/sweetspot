﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Microsoft.Xna.Framework;

namespace SweetSpot.Database
{
    public enum PersistenceStrategy { Immediate, Cached }

    class SQLiteAdapter : IDatabase
    {
        const string filename = "database.sqlite";
        const string tableCalibration = "calibration";
        const string tableSweetSpotBounds = "sweetspot_bounds";
        const string tableTest = "test";
        const string tableUserPosition = "user_position";
        const int cacheSize = 1000;

        protected string db;
        protected List<string> insertCache;

        public SQLiteAdapter()
        {
            db = "Data Source=" + filename;
            insertCache = new List<string>();
        }

        ~SQLiteAdapter()
        {
            flushInsertCache();
        }

        public bool HasCalibrationDataFor(string deviceID)
        {
            string sql = String.Format("SELECT COUNT(*) FROM {0} WHERE device_id='{1}';", tableCalibration, deviceID);
            string result = ExecuteScalarQuery(sql);

            return int.Parse(result) > 0;
        }

        public Tuple<float, Vector3> LoadCalibration(string deviceID)
        {
            string sql = String.Format("SELECT * FROM {0} WHERE device_id='{1}' ORDER BY created DESC;", tableCalibration, deviceID);
            DataTable table = ExecuteTableQuery(sql);
            
            if (table.Rows.Count == 0)
                return new Tuple<float, Vector3>(0f, new Vector3());

            DataRow row = table.Rows[0];
            float axisTilt = float.Parse(row["axis_tilt"].ToString());
            float x = float.Parse(row["translate_x"].ToString());
            float y = float.Parse(row["translate_y"].ToString());
            float z = float.Parse(row["translate_z"].ToString());

            return new Tuple<float, Vector3>(axisTilt, new Vector3(x, y, z));
        }

        public void SaveCalibration(string deviceID, float axisTilt, Vector3 translate)
        {
            string sql = String.Format("DELETE FROM {0} WHERE device_id='{1}';", tableCalibration, deviceID);
            ExecuteNonQuery(sql);

            var data = new Dictionary<string, string>
            {
                {"device_id", deviceID},
                {"axis_tilt", axisTilt.ToString()},
                {"translate_x", translate.X.ToString()},
                {"translate_y", translate.Y.ToString()},
                {"translate_z", translate.Z.ToString()}
            };
            Insert(tableCalibration, data);
        }

        public List<Vector2> LoadSweetSpots()
        {
            string sql = String.Format("SELECT x, y FROM {0};", tableSweetSpotBounds);
            DataTable table = ExecuteTableQuery(sql);
            var sweetSpots = new List<Vector2>();
            foreach (DataRow row in table.Rows)
            {
                float x = float.Parse(row["x"].ToString());
                float y = float.Parse(row["y"].ToString());
                sweetSpots.Add(new Vector2(x, y));
            }

            return sweetSpots;
        }

        public void SaveSweetSpot(Vector2 sweetSpot)
        {
            var sweetSpots = new Dictionary<string, string>
            {
                {"x", sweetSpot.X.ToString()},
                {"y", sweetSpot.Y.ToString()}
            };
            Insert(tableSweetSpotBounds, sweetSpots);
        }

        public int GetNewSubjectID()
        {
            string sql = String.Format("SELECT COUNT(*) FROM {0};", tableTest);
            string result = ExecuteScalarQuery(sql);
            if (int.Parse(result) == 0)
                return 1;

            sql = String.Format("SELECT MAX(subject) FROM {0};", tableTest);
            result = ExecuteScalarQuery(sql);
            return int.Parse(result) + 1;
        }

        public int GetNewTestID()
        {
            string sql = String.Format("SELECT COUNT(*) FROM {0};", tableTest);
            string result = ExecuteScalarQuery(sql);
            return int.Parse(result) + 1;
        }

        public int RecordTest(int testSubject, string cue, Vector2 sweetSpot)
        {
            int testID = GetNewTestID();

            var test = new Dictionary<string, string>
            {
                {"id", testID.ToString()},
                {"subject", testSubject.ToString()},
                {"cue", cue},
                {"sweetspot_x", sweetSpot.X.ToString()},
                {"sweetspot_y", sweetSpot.Y.ToString()}
            };
            Insert(tableTest, test);

            return testID;
        }

        public void RecordUserPosition(int test, Vector2 position, int timestamp)
        {
            var positionRecord = new Dictionary<string, string>
            {
                {"test_id", test.ToString()},
                {"timestamp", timestamp.ToString()},
                {"x", position.X.ToString()},
                {"y", position.Y.ToString()}
            };
            Insert(tableUserPosition, positionRecord, PersistenceStrategy.Cached);
        }

        public void ExecuteCachedNonQuery(string sql)
        {
            insertCache.Add(sql);
        }

        public void ExecuteNonQuery(string sql)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(db);
                connection.Open();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                Logger.Log(String.Format("Failed to execute non-query: \"{0}\"", sql));
            }
        }

        public void ExecuteNonQueries(List<string> sqls)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(db);
                connection.Open();
                SQLiteCommand command = connection.CreateCommand();
                SQLiteTransaction transaction = connection.BeginTransaction();
                foreach (string sql in sqls)
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
                connection.Close();
                command.Dispose();
                connection.Dispose();
            }
            catch (Exception)
            {
                Logger.Log("Failed to execute batch query:");
                foreach (string sql in sqls)
                {
                    Logger.Log("    " + sql);
                }
            }
        }

        public string ExecuteScalarQuery(string sql)
        {
            flushInsertCache();
            try
            {
                SQLiteConnection connection = new SQLiteConnection(db);
                connection.Open();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = sql;
                object result = command.ExecuteScalar();
                connection.Close();
                return result.ToString();
            }
            catch (Exception e)
            {
                Logger.Log(String.Format("Failed to execute scalar query: \"{0}\"", sql));
                throw e;
            }
        }

        public DataTable ExecuteTableQuery(string sql)
        {
            flushInsertCache();
            DataTable table = new DataTable();
            try
            {
                SQLiteConnection connection = new SQLiteConnection(db);
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                SQLiteDataReader reader = command.ExecuteReader();
                table.Load(reader);
                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.Log(String.Format("Failed to execute table query: \"{0}\"", sql));
                throw e;
            }
            return table;
        }

        public void Insert(string tableName, Dictionary<string, string> data, PersistenceStrategy caching = PersistenceStrategy.Immediate)
        {
            string columns = "";
            string values = "";
            foreach (KeyValuePair<string, string> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            string sql = String.Format("INSERT INTO {0}({1}) VALUES({2});", tableName, columns, values);

            switch (caching)
            {
                case PersistenceStrategy.Cached:
                    ExecuteCachedNonQuery(sql);
                    break;
                case PersistenceStrategy.Immediate:
                    ExecuteNonQuery(sql);
                    break;
            }
        }

        public void ClearTable(string tableName)
        {
            ExecuteNonQuery(String.Format("DELETE FROM {0};", tableName));
        }

        protected void cacheQuery(string sql)
        {
            insertCache.Add(sql);
            if (insertCache.Count > cacheSize)
                flushInsertCache();
        }

        protected void flushInsertCache()
        {
            ExecuteNonQueries(insertCache);
            insertCache.Clear();
        }
    }
}
