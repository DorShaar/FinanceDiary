using ConsoleTables;

namespace FinanceDiary.Client
{
    public class ConsoleUI
    {
        public void Print()
        {
            ConsoleTable consoleTable = new ConsoleTable("1", "2", "3");
            consoleTable.AddRow("abc", "cfg", "dfdf");
            consoleTable.AddRow("ab555c", "cf3434g", "dfd2131231");

            consoleTable.Write(Format.Alternative);
        }
    }
}