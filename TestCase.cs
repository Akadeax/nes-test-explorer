namespace NESTestExplorer;

internal class TestCase
{
    public List<bool> Checks = [];
    public string Name = "";

    public TestCase() { }

    public TestCase(TestCase other)
    {
        Checks = [.. other.Checks];
    }

    public int SucceededAmount => Checks.Count(b => b == true);
    public int FailedAmount => Checks.Count(b => b == false);

    public bool AnyFailed => Checks.Any(b => b == false);
}
