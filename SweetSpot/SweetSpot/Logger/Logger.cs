using System;
using System.IO;
using Sweetspot.Util;

namespace Sweetspot
{
    public static class Logger
    {
        const string FILENAME = "sweetspot.log";

        public static void Log(string msg)
        {
            using (StreamWriter file = new StreamWriter(FILENAME, true))
            {
                file.WriteLine(String.Format("[{0}] {1}", DateTime.Now.GetTimestamp(), msg));
            }
        }
    }
}
