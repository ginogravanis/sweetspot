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
            FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter file = new StreamWriter(stream);
            file.WriteLine(String.Format("[{0}] {1}", DateTime.Now.GetTimestamp(), msg));
            file.Close();
            stream.Close();
        }
    }
}
