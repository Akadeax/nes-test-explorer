using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace NESTestExplorer;

internal class NESTestExplorer
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            args = [
                @"mesen --testrunner E:\dev\Programming\NES\nespad\build\Nespad.nes E:\dev\Programming\NES\nespad/scripts/tests/tests_adapter.lua",
                @"E:\dev\Programming\NES\nespad\src\tests\tests.s",
            ];
        }

        string startSenderCommand = args[0];
        string testsFilePath = args[1];

        var receiverTask = TestDataReceiver.Receive(startSenderCommand);
        var parserTask = TestsFileParser.ReadAndParse(testsFilePath);

        await Task.WhenAll(receiverTask, parserTask);

        var categoryList = await receiverTask;
        var nameList = await parserTask;

        int totalNamesRequired = categoryList.Sum(cat => cat.Cases.Count) + categoryList.Count;

        if (totalNamesRequired != nameList.Count)
        {
            Formatting.SendFormatError($"Name mismatch: receiver provided {totalNamesRequired} names, but parser provided {nameList.Count}!");
            return 1;
        }

        int nameIndex = 0;
        foreach (Category cat in categoryList)
        {
            cat.Name = nameList[nameIndex++];

            foreach (TestCase currentCase in cat.Cases)
            {
                currentCase.Name = nameList[nameIndex++];
            }
        }

        OutputFormatter.Output(categoryList);

        return 0;
    }
}
