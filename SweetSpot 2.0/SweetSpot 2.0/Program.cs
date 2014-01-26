using System;

namespace SweetSpot_2._0
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (SweetSpot app = new SweetSpot())
            {
                app.Run();
            }
        }
    }
#endif
}

