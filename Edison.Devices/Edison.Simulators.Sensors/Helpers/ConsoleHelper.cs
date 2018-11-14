using System;

namespace Edison.Simulators.Sensors.Helpers
{
    public static class ConsoleHelper
    {
        public static void WriteInfo(string message)
        {
            Console.WriteLine(message);
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteHighlight(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ClearConsole()
        {
            Console.Clear();
        }
    }
}
