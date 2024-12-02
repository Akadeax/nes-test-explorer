using System.Security.Cryptography;

namespace NESTestExplorer;

internal class OutputFormatter
{
    public static void Output(List<Category> categoryList)
    {
        const int PAD = 40;
        const char PAD_CHAR = '=';

        foreach ((Category category, int categoryIndex) in categoryList.WithIndex())
        {
            Console.WriteLine("".PadRight(PAD, PAD_CHAR));
            // === Category name (Nr. index):\n
            Console.Write("==== Category ");
            Formatting.WriteInColor($"{category.Name} (Nr. {categoryIndex + 1})", ConsoleColor.Cyan);
            Console.WriteLine(":");

            foreach (TestCase testCase in category.Cases)
            {
                // = Case name
                Console.Write($"== Case ");
                Formatting.WriteInColor(testCase.Name, ConsoleColor.Blue);

                if (testCase.AnyFailed)
                {
                    //  (x SUCCESS, y FAIL):
                    Console.Write(" (");
                    Formatting.WriteInColor($"{testCase.SucceededAmount} Succeeded", ConsoleColor.Green);
                    Console.Write(", ");
                    Formatting.WriteInColor($"{testCase.FailedAmount} Failed", ConsoleColor.Red);
                    Console.WriteLine("):");

                    foreach ((bool check, int checkIndex) in testCase.Checks.WithIndex())
                    {
                        Console.Write($"---> Check {checkIndex}: ");
                        if (check)
                        {
                            Formatting.WriteLineInColor("Success", ConsoleColor.Green);
                        }
                        else
                        {
                            Formatting.WriteLineInColor("Fail", ConsoleColor.Red);
                        }
                    }
                }
                else
                {
                    Console.Write(" (");
                    Formatting.WriteInColor($"{testCase.SucceededAmount} Succeeded", ConsoleColor.Green);
                    Console.WriteLine(")");
                }
            }
        }

        Console.WriteLine("".PadRight(PAD, PAD_CHAR));

        int totalSuccess = categoryList.Sum(cat => cat.SucceededAmount);
        int totalFailure = categoryList.Sum(cat => cat.FailedAmount);

        // Test Result: x Succeeded, y Failed
        Console.Write("====== Test Result: ");
        Formatting.WriteInColor($"{totalSuccess} Succeeded", ConsoleColor.Green);

        Console.Write(", ");

        Formatting.WriteLineInColor($"{totalFailure} Failed", ConsoleColor.Red);

        Console.WriteLine("".PadRight(PAD, PAD_CHAR));
    }
}
