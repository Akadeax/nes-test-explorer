namespace NESTestExplorer;

public class TestsFileParser
{
    public static async Task<List<(string, int)>> ReadAndParse(string testsFilePath)
    {
        string[] fileLines;
        try
        {
            fileLines = await File.ReadAllLinesAsync(testsFilePath);
        }
        catch (ArgumentException)
        {
            Formatting.SendFormatError($"Failed to read tests file at '{testsFilePath}'!");
            return [];
        }

        List<(string, int)> names = [];

        foreach (var (line, index) in fileLines.WithIndex())
        {
            string lineTrim = line.Trim();
            if (!lineTrim.StartsWith("__CATEGORY__") && !lineTrim.StartsWith("__CASE__"))
            {
                continue;
            }

            int firstSemicolonIndex = lineTrim.IndexOf(';');
            if (firstSemicolonIndex == -1)
            {
                Formatting.SendFormatError($"Line {index}: Element is unnamed!");
                return [];
            }

            string currentName = lineTrim[(firstSemicolonIndex + 1)..].Trim();
            names.Add((currentName, index + 1));
        }

        return names;
    }
}