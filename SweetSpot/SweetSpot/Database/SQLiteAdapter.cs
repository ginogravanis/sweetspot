using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Microsoft.Xna.Framework;
using SweetSpot.Input;
using SweetSpot.ScreenManagement;
using SweetSpot.Util;

namespace SweetSpot.Database
{
    public enum PersistenceStrategy { Immediate, Buffered }

    class SQLiteAdapter : IDatabase, ICalibrationProvider
    {
        const string FILENAME = "database.sqlite";
        const string TABLE_CALIBRATION = "calibration";
        const string TABLE_SWEETSPOT_BOUNDS = "sweetspot_bounds";
        const string TABLE_TEST = "test";
        const string TABLE_USER_POSITION = "user_position";
        const string TABLE_QUESTIONS = "questions";

        protected string db;
        protected List<string> insertBuffer;

        protected int lastQuestionId = 0;

        public SQLiteAdapter()
        {
            db = "Data Source=" + FILENAME;
            insertBuffer = new List<string>();
        }

        ~SQLiteAdapter()
        {
            flushInsertBuffer();
        }

        public bool HasCalibrationDataFor(string deviceID)
        {
            string sql = String.Format("SELECT COUNT(*) FROM {0} WHERE device_id='{1}';", TABLE_CALIBRATION, deviceID);
            string result = ExecuteScalarQuery(sql);

            return int.Parse(result) > 0;
        }

        public Tuple<float, Vector3> LoadCalibration(string deviceID)
        {
            string sql = String.Format("SELECT * FROM {0} WHERE device_id='{1}' ORDER BY created DESC;", TABLE_CALIBRATION, deviceID);
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
            string sql = String.Format("DELETE FROM {0} WHERE device_id='{1}';", TABLE_CALIBRATION, deviceID);
            ExecuteNonQuery(sql);

            var data = new Dictionary<string, string>
            {
                {"device_id", deviceID},
                {"axis_tilt", axisTilt.ToString()},
                {"translate_x", translate.X.ToString()},
                {"translate_y", translate.Y.ToString()},
                {"translate_z", translate.Z.ToString()}
            };
            Insert(TABLE_CALIBRATION, data);
        }

        public IEnumerable<Vector2> LoadSweetSpotBounds()
        {
            string sql = String.Format("SELECT x, y FROM {0};", TABLE_SWEETSPOT_BOUNDS);
            DataTable table = ExecuteTableQuery(sql);
            var sweetSpotBounds = new List<Vector2>();
            foreach (DataRow row in table.Rows)
            {
                float x = float.Parse(row["x"].ToString());
                float y = float.Parse(row["y"].ToString());
                sweetSpotBounds.Add(new Vector2(x, y));
            }

            return sweetSpotBounds;
        }

        public void SaveSweetSpotBounds(IEnumerable<Vector2> sweetSpotBounds)
        {
            foreach (var point in sweetSpotBounds)
            {
                var points = new Dictionary<string, string>
                {
                    {"x", point.X.ToString()},
                    {"y", point.Y.ToString()}
                };
                Insert(TABLE_SWEETSPOT_BOUNDS, points);
            }
        }

        public int GetNewSubjectID()
        {
            string sql = String.Format("SELECT COALESCE(MAX(subject), 0) FROM {0};", TABLE_TEST);
            string maxSubjectID = ExecuteScalarQuery(sql);
            return int.Parse(maxSubjectID) + 1;
        }

        public int GetNewTestID()
        {
            string sql = String.Format("SELECT COALESCE(MAX(id), 0) FROM {0};", TABLE_TEST);
            string maxTestID = ExecuteScalarQuery(sql);
            return int.Parse(maxTestID) + 1;
        }

        public int RecordTest(int testSubject, string cue, Mapping mapping)
        {
            return RecordTest(testSubject, cue, mapping, new Vector2());
        }

        public int RecordTest(int testSubject, string cue, Mapping mapping, Vector2 sweetSpot)
        {
            int testID = GetNewTestID();

            var test = new Dictionary<string, string>
            {
                {"id", testID.ToString()},
                {"subject", testSubject.ToString()},
                {"cue", cue},
                {"mapping", mapping.ToString()},
                {"sweetspot_x", sweetSpot.X.ToString()},
                {"sweetspot_y", sweetSpot.Y.ToString()},
                {"begin_ms", DateTime.Now.Millisecond.ToString()}
            };
            Insert(TABLE_TEST, test);

            return testID;
        }

        public QuizItem GetQuestion()
        { 
            string sql = String.Format("SELECT * FROM {0} WHERE ID > {1} OR ID= (SELECT MIN(ID) FROM {0}) LIMIT 1;",
                TABLE_QUESTIONS, lastQuestionId);
            DataTable table = ExecuteTableQuery(sql);

            if (table.Rows.Count == 0) 
                throw new ApplicationException("Cant find any questions");

            DataRow row = table.Rows[0];
            string question = row["questions"].ToString();
            string answer = row["answer"].ToString();
            lastQuestionId = Int32.Parse(row["id"].ToString());

            return new QuizItem(question, answer);
        }

        public void TestCompleted(int test, int timestamp)
        {
            ExecuteNonQuery(String.Format("UPDATE {0} SET task_completed={1} WHERE id={2};", TABLE_TEST, timestamp, test));
            flushInsertBuffer();
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
            Insert(TABLE_USER_POSITION, positionRecord, PersistenceStrategy.Buffered);
        }

        public void ExecuteBufferedNonQuery(string sql)
        {
            insertBuffer.Add(sql);
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

        public void ExecuteNonQueryBatch(IEnumerable<string> sqls)
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
                case PersistenceStrategy.Buffered:
                    ExecuteBufferedNonQuery(sql);
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

        protected void bufferQuery(string sql)
        {
            insertBuffer.Add(sql);
        }

        protected void flushInsertBuffer()
        {
            ExecuteNonQueryBatch(insertBuffer);
            insertBuffer.Clear();
        }
    }
}
