using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NESTestExplorer;

internal class Formatting
{
    public static void WriteLineInColor(string str, ConsoleColor color, bool reset = true)
    {
        WriteInColor($"{str}\n", color, reset);
    }

    public static void WriteInColor(string str, ConsoleColor color, bool reset = true)
    {
        ConsoleColor preColor = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.Write(str);

        if (reset)
        {
            Console.ForegroundColor = preColor;
        }
    }

    public static void SendFormatError(string str)
    {
        WriteLineInColor($"[ERROR] {str}", ConsoleColor.Red);
        Environment.Exit(1);
    }

    public static void SendFormatWarning(string str)
    {
        WriteLineInColor($"[WARN] {str}", ConsoleColor.Yellow);
    }
}
