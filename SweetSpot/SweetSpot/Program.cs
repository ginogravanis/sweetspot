namespace SweetSpot
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

