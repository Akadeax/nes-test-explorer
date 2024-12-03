using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace NESTestExplorer;

internal class TestDataReceiver
{
    public static async Task<List<Category>> Receive(string startSenderCommand)
    {
        Run(startSenderCommand);

        string totalTestOutput;
        try
        {
            totalTestOutput = await ListenForTestOutput();
        }
        catch (TimeoutException)
        {
            Formatting.SendFormatError("Receiving test data has timed out!");
            return [];
        }
        catch (ArgumentException e)
        {
            Formatting.SendFormatError(e.Message);
            return [];
        }

        Console.WriteLine($"Raw Data: {totalTestOutput}");

        List<Category> categories;
        try
        {
            categories = ParseIntoCategories(totalTestOutput);
        }
        catch (ArgumentException e)
        {
            Formatting.SendFormatError(e.Message);
            return [];
        }

        return categories;
    }

    static async Task<string> ListenForTestOutput()
    {
        TcpListener server = new TcpListener(IPAddress.Loopback, 4065);

        server.Start();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Waiting for test data...");

        // This throws TimeoutException if timeout is reached
        TcpClient client = await server.AcceptTcpClientAsync().WaitAsync(TimeSpan.FromSeconds(1));

        NetworkStream ns = client.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead;

        string totalTestOutput = "";
        while ((bytesRead = ns.Read(buffer, 0, buffer.Length)) > 0)
        {
            totalTestOutput += Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        client.Close();


        if (totalTestOutput.Length == 0)
        {
            throw new ArgumentException("Test data not found!");
        }

        // Account for leading 0s caused by memory clears prior to running tests
        while (totalTestOutput[0] == '0')
        {
            totalTestOutput = totalTestOutput[1..];
        }

        if (totalTestOutput[0] != Constants.SENDER_VAL_CATEGORY)
        {
            throw new ArgumentException($"Test data does not start with category! Data: {totalTestOutput}");
        }

        return totalTestOutput[1..];
    }


    static void Run(string command)
    {
        Process process = new Process();

        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $"/c {command}";

        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
    }


    static List<Category> ParseIntoCategories(string testOutput)
    {
        List<string> categoryStringList = [];
        string currentCategory = "";

        foreach (char ch in testOutput)
        {
            switch (ch)
            {
                case Constants.SENDER_VAL_CATEGORY:
                    if (currentCategory == "")
                    {
                        throw new ArgumentException("Category has no cases!");
                    }

                    categoryStringList.Add(currentCategory);
                    currentCategory = "";
                    break;

                default:
                    currentCategory += ch;
                    break;
            }
        }

        if (currentCategory == "")
        {
            throw new ArgumentException("Category has no cases!");
        }

        categoryStringList.Add(currentCategory);

        List<Category> categories = [];
        foreach (string catString in categoryStringList)
        {
            try
            {
                categories.Add(Category.FromString(catString));
            }
            catch (ArgumentException e)
            {
                Formatting.SendFormatError(e.Message);
                return [];
            }
        }

        return categories;
    }
}