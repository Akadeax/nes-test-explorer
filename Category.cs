namespace NESTestExplorer;

internal class Category
{
    public List<TestCase> Cases = [];
    public string Name = "";
    public int Line = -1;

    public int SucceededAmount => Cases.Sum(c => c.SucceededAmount);
    public int FailedAmount => Cases.Sum(c => c.FailedAmount);

    public bool AnyFailed => Cases.Any(c => c.AnyFailed);

    public static Category FromString(string str)
    {
        if (str.Length == 0)
        {
            return new Category();
        }

        if (str[0] != Constants.SENDER_VAL_CASE)
        {
            throw new ArgumentException("Category does not start with case!");
        }

        // remove required trailing CASE
        str = str[1..];

        Category category = new Category();

        TestCase? currentCase = new TestCase();

        foreach (char ch in str)
        {
            switch (ch)
            {
                case Constants.SENDER_VAL_CASE:
                    if(currentCase.Checks.Count == 0)
                    {
                        throw new ArgumentException("Case has no checks!");
                    }

                    category.Cases.Add(new TestCase(currentCase));
                    currentCase = new TestCase();
                    break;

                default:
                    currentCase.Checks.Add(ch == Constants.SENDER_VAL_SUCCESS);
                    break;
            }
        }

        if (currentCase.Checks.Count == 0)
        {
            throw new ArgumentException("Case has no checks!");
        }

        category.Cases.Add(currentCase);

        return category;
    }
}
