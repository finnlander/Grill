using System;

namespace Grill.Lib.Helpers
{
    public static class ConsoleWriter
    {
        /* Constants */
        private const ConsoleColor ColorDebugMessage = ConsoleColor.DarkGray;
        private const ConsoleColor ColorInfoMessage = ConsoleColor.Gray;
        private const ConsoleColor ColorImportantMessage = ConsoleColor.Green;
        private const ConsoleColor ColorErrorMessage = ConsoleColor.Red;
        private const ConsoleColor ColorWarningMessage = ConsoleColor.DarkYellow;

        /* Public Methods */
        public static void ImportantInfo(string msg, ConsoleColor color= ColorImportantMessage, bool surroundWithEmptyLines=true)
        {
            WriteConsoleLine(msg, ColorImportantMessage, surroundWithEmptyLines);
        }

        public static void Info(string highlightedMessage, string nonHighlightedMessage, ConsoleColor highlightColor, bool surroundWithEmptyLines = true)
        {
            if (surroundWithEmptyLines)
                Console.WriteLine();
            WriteConsole(highlightedMessage, highlightColor);
            WriteConsoleLine(nonHighlightedMessage, ColorInfoMessage, false);

            if (surroundWithEmptyLines)
                Console.WriteLine();
        }

        public static void Info(string msg, bool surroundWithEmptyLines = false)
        {
            WriteConsoleLine(msg, ColorInfoMessage, surroundWithEmptyLines);
        }

        public static void Error(string msg, bool surroundWithEmptyLines = true)
        {
            WriteConsoleLine(msg, ColorErrorMessage, surroundWithEmptyLines);
        }

        public static void Warn(string msg, bool surroundWithEmptyLines = true)
        {
            WriteConsoleLine(msg, ColorWarningMessage, surroundWithEmptyLines);
        }


        public static void Debug(string msg, bool surroundWithEmptyLines = false)
        {
            WriteConsoleLine(msg, ColorDebugMessage, surroundWithEmptyLines);
        }

        /* Private Methods */
        private static void WriteConsole(string msg, ConsoleColor color)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ResetColor();
        }
        private static void WriteConsoleLine(string msg, ConsoleColor color, bool surroundWithEmptyLines)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                Console.WriteLine();
                return;
            }

            if (surroundWithEmptyLines)
                Console.WriteLine();
            Console.ForegroundColor = color;

            Console.WriteLine(msg);
            if (surroundWithEmptyLines)
                Console.WriteLine();
            Console.ResetColor();
        }

    }
}
