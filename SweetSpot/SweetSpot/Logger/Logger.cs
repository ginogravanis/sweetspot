using System;
using System.IO;
using SweetSpot.Util;

namespace SweetSpot
{
    public static class Logger
    {
        const string fileName = "sweetspot.log";

        public static void Log(string msg)
        {
            using (StreamWriter file = new StreamWriter(fileName, true))
            {
                file.WriteLine(String.Format("[{0}] {1}", DateTime.Now.GetTimestamp(), msg));
            }
        }
    }
}
