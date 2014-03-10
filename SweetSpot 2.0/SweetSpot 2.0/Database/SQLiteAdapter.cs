using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0.Database
{
    class SQLiteAdapter : IDatabase
    {
        const string filename = "database.sqlite";

        protected string db;

        public SQLiteAdapter()
        {
            db = "Data Source=" + filename;
        }

        public bool HasCalibrationDataFor(string deviceID)
        {
            string sql = String.Format("SELECT COUNT(*) FROM calibration WHERE deviceId='{0}';", deviceID);
            string result = ExecuteScalarQuery(sql);

            return int.Parse(result) > 0;
        }

        public Tuple<float, Vector3> LoadCalibration(string deviceID)
        {
            string sql = String.Format("SELECT MAX(rowid) FROM calibration WHERE deviceID='{0}';", deviceID);
            int rowid = int.Parse(ExecuteScalarQuery(sql));

            sql = String.Format("SELECT axis_tilt FROM calibration WHERE rowid='{0}';", rowid);
            float axisTilt = float.Parse(ExecuteScalarQuery(sql));

            sql = String.Format("SELECT translate_x FROM calibration WHERE rowid='{0}';", rowid);
            float x = float.Parse(ExecuteScalarQuery(sql));

            sql = String.Format("SELECT translate_y FROM calibration WHERE rowid='{0}';", rowid);
            float y = float.Parse(ExecuteScalarQuery(sql));

            sql = String.Format("SELECT translate_z FROM calibration WHERE rowid='{0}';", rowid);
            float z = float.Parse(ExecuteScalarQuery(sql));

            return new Tuple<float, Vector3>(axisTilt, new Vector3(x, y, z));
        }

        public void SaveCalibration(string deviceID, float axisTilt, Vector3 translate)
        {
            string tableName = "calibration";

            ExecuteNonQuery(String.Format("DELETE FROM {0} WHERE deviceId='{1}'", tableName, deviceID));

            var data = new Dictionary<string, string>
            {
                {"deviceId", deviceID},
                {"axis_tilt", axisTilt.ToString()},
                {"translate_x", translate.X.ToString()},
                {"translate_y", translate.Y.ToString()},
                {"translate_z", translate.Z.ToString()}
            };
            Insert(tableName, data);
        }

        public List<Vector2> LoadSweetSpots()
        {
            throw new NotImplementedException();
        }

        public void SaveSweetSpot(int id, Vector2 sweetSpot)
        {
            string tableName = "sweetspot";

            var sweetSpots = new Dictionary<string, string>
            {
                {"id", id.ToString()},
                {"x", sweetSpot.X.ToString()},
                {"y", sweetSpot.Y.ToString()}
            };
            Insert(tableName, sweetSpots);
        }

        public int RecordTest(int testSubject, string screen, int sweetSpot)
        {
            throw new NotImplementedException();
        }

        public void RecordUserPosition(int test, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public void ExecuteNonQuery(string sql)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(db);
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                // TODO: write to logfile
            }
        }

        public string ExecuteScalarQuery(string sql)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(db);
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                object result = command.ExecuteScalar();
                connection.Close();
                return result.ToString();
            }
            catch (Exception e)
            {
                // TODO: write to logfile
                throw e;
            }
        }

        public DataTable ExecuteTableQuery(string sql)
        {
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
                // TODO: write to logfile
                throw e;
            }
            return table;
        }

        public void Insert(string tableName, Dictionary<string, string> data)
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
            ExecuteNonQuery(String.Format("INSERT INTO {0}({1}) VALUES({2});", tableName, columns, values));
        }

        public void ClearTable(string tableName)
        {
            ExecuteNonQuery(String.Format("DELETE FROM {0};", tableName));
        }
    }
}
