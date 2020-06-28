namespace FinanceDiary.Infra.CommandParsers
{
    public interface ICommandParser
    {
        string[] Parse(string input);
    }
}