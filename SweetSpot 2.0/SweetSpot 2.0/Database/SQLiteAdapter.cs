using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0.Database
{
    class SQLiteAdapter : IDatabase
    {
        const string filename = "database.sqlite";

        protected SQLiteConnection connection;

        public bool HasCalibrationDataFor(string deviceID)
        {
            throw new NotImplementedException();
        }

        public Tuple<float, Vector3> GetCalibration(string deviceID)
        {
            throw new NotImplementedException();
        }

        public SQLiteAdapter()
        {
            connection = new SQLiteConnection("Data Source=" + filename);
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

        public void SaveSweetSpots()
        {
            throw new NotImplementedException();
        }

        public void GetSweetSpots()
        {
            throw new NotImplementedException();
        }

        public void RecordUserPosition()
        {
            throw new NotImplementedException();
        }

        public void ExecuteNonQuery(string sql)
        {
            try
            {
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
