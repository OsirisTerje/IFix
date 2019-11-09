using System;

namespace IFix
{
    public static class Writer
    {
        public static void WriteRed(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void WriteGreen(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Write(string msg)
        {
            Console.WriteLine(msg);
        }


    }
}
