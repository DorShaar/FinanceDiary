using CommandLine;

namespace FinanceDiary.Infra.ClientApi
{
    internal class CommandLineOptions
    {
        [Verb("withdraw", HelpText = "withdraw <amount> -d <date> -k <kind> -r <reason>")]
        public class WithdrawOptions
        {
            [Value(0, HelpText = "Amount")]
            public int Amount { get; set; }

            [Option('d', "date", HelpText = "Date", Required = true)]
            public string Date { get; set; }

            [Option('k', "kinds", HelpText = "Operation kinds", Required = true)]
            public string Kinds { get; set; }

            [Option('r', "reason", HelpText = "Operation kind", Required = true)]
            public string Reason { get; set; }
        }

        [Verb("deposit", HelpText = "deposit <amount> -d <date> -k <kind> -r <reason>")]
        public class DepositOptions
        {
            [Value(0, HelpText = "Amount")]
            public int Amount { get; set; }

            [Option('d', "date", HelpText = "Date", Required = true)]
            public string Date { get; set; }

            [Option('k', "kinds", HelpText = "Operation kinds", Required = true)]
            public string Kinds { get; set; }

            [Option('r', "reason", HelpText = "Operation kind", Required = true)]
            public string Reason { get; set; }
        }

        [Verb("move", HelpText = "move <amount> -d <date> -s <source-cash-register> -d <destination-cash-register> -r <reason>")]
        public class MoveOptions
        {
            [Value(0, HelpText = "Amount")]
            public int Amount { get; set; }

            [Option('d', "date", HelpText = "Date", Required = true)]
            public string Date { get; set; }
             
            [Option('s', "source", HelpText = "Source cash register", Required = true)]
            public string SourceCashRegister { get; set; }

            [Option('t', "dest", HelpText = "Destination cash register", Required = true)]
            public string DestinationCashRegister { get; set; }

            [Option('r', "reason", HelpText = "Operation kind", Required = true)]
            public string Reason { get; set; }
        }

        [Verb("register", HelpText = "register <cash-register-name> <initial-amount>")]
        public class RegsiterCashOptions
        {
            [Value(0, HelpText = "Name")]
            public string Name { get; set; }

            [Value(1, HelpText = "InitialAmount")]
            public int InitialAmount { get; set; }
        }

        [Verb("get", HelpText = "get (cash-register, report)")]
        public class GetOptions
        {
            [Value(0, HelpText = "Object type (cash-register, report)")]
            public string ObjectType { get; set; }
        }

        [Verb("save", HelpText = "save database")]
        public class SaveOptions
        {
        }
    }
}