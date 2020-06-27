namespace FinanceDiary.Infra.ClientApi
{
    public interface ICommandLineRunner
    {
        public int RunCommand(string[] args);
    }
}