using CommandLine;
using ConsoleTables;
using FinanceDiary.App;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDiary.Infra.ClientApi
{
    public class CommandLineParserAdapter : ICommandLineRunner
    {
        private readonly IFinanceDiaryManager mFinanceDiaryManager;
        private readonly ILogger<CommandLineParserAdapter> mLogger;

        public CommandLineParserAdapter(IFinanceDiaryManager financeDiaryManager, ILogger<CommandLineParserAdapter> logger)
        {
            mFinanceDiaryManager = financeDiaryManager;
            mLogger = logger;
        }

        public int RunCommand(string[] args)
        {
            using Parser parser = new Parser(config => config.HelpWriter = Console.Out);
            if (args.Length == 0)
            {
                parser.ParseArguments<CommandLineOptions>(new[] { "--help" });
                return 1;
            }

            return ParseArgument(parser, args);
        }

        private int ParseArgument(Parser parser, string[] args)
        {
            return parser.ParseArguments<
                CommandLineOptions.DepositOptions,
                CommandLineOptions.WithdrawOptions,
                CommandLineOptions.MoveOptions,
                CommandLineOptions.RegsiterCashOptions,
                CommandLineOptions.GetOptions,
                CommandLineOptions.SaveOptions>(args).MapResult(
                    (CommandLineOptions.DepositOptions options) => Deposit(options),
                    (CommandLineOptions.WithdrawOptions options) => Withdraw(options),
                    (CommandLineOptions.MoveOptions options) => Move(options),
                    (CommandLineOptions.RegsiterCashOptions options) => RegisterCash(options),
                    (CommandLineOptions.GetOptions options) => Print(options),
                    (CommandLineOptions.SaveOptions options) => Save(options),
                    (parserErrors) => 1
                );
        }

        private int Deposit(CommandLineOptions.DepositOptions options)
        {
            List<OperationKind> operationKinds = ConvertStringToOperationKinds(options.Kinds);

            if (!mFinanceDiaryManager.AddFinanceOperation(
                options.Date,
                OperationType.Deposit,
                options.Amount,
                operationKinds,
                options.Reason))
            {
                mLogger.LogWarning($"Deposit operation of {options.Amount} at {options.Date} of kind {options.Kinds} " +
                    $"and reason {options.Reason} failed");
                return 1;
            }

            return 0;
        }

        private int Withdraw(CommandLineOptions.WithdrawOptions options)
        {
            List<OperationKind> operationKinds = ConvertStringToOperationKinds(options.Kinds);

            if (!mFinanceDiaryManager.AddFinanceOperation(
                options.Date,
                OperationType.Withdraw,
                options.Amount,
                operationKinds,
                options.Reason))
            {
                mLogger.LogWarning($"Deposit operation of {options.Amount} at {options.Date} of kind {options.Kinds} " +
                    $"and reason {options.Reason} failed");
                return 1;
            }

            return 0;
        }

        private List<OperationKind> ConvertStringToOperationKinds(string kindsString)
        {
            List<OperationKind> operationKinds = new List<OperationKind>();

            try
            {
                string[] kinds = kindsString.Split(',');
                foreach (string kind in kinds)
                {
                    operationKinds.Add((OperationKind)Enum.Parse(typeof(OperationKind), kind));
                }
            }
            catch(Exception ex)
            {
                mLogger.LogWarning(ex, "Could not parse operation kind strings");
                return new List<OperationKind>();
            }

            return operationKinds;
        }

        private int Move(CommandLineOptions.MoveOptions options)
        {
            if (!mFinanceDiaryManager.AddNeutralOperation(
                options.Date,
                options.Amount,
                options.SourceCashRegister,
                options.DestinationCashRegister,
                options.Reason))
            {
                mLogger.LogWarning($"Neutral operation of {options.Amount} at {options.Date} " +
                    $"from {options.SourceCashRegister} to {options.DestinationCashRegister}" +
                    $"and reason {options.Reason} failed");
                return 1;
            }

            return 0;
        }

        private int RegisterCash(CommandLineOptions.RegsiterCashOptions options)
        {
            if (!mFinanceDiaryManager.AddCashRegister(
                options.Name,
                options.InitialAmount))
            {
                mLogger.LogWarning($"Registration of new cash register {options.Name} with " +
                    $"initial amount {options.InitialAmount} failed");
                return 1;
            }

            return 0;
        }

        private int Print(CommandLineOptions.GetOptions options)
        {
            try
            {
                if (IsReferringCashRegister(options.ObjectType))
                {
                    PrintCashRegisterStatus();
                    return 0;
                }

                mLogger.LogWarning($"Object type {options.ObjectType} did not matched");
                return 1;
            }
            catch (Exception ex)
            {
                mLogger.LogWarning(ex, $"Error on print {options.ObjectType}");
                return 1;
            }
        }

        private bool IsReferringCashRegister(string userInput)
        {
            string lowerUserInput = userInput.ToLowerInvariant();
            return
                lowerUserInput.Equals("cash") ||
                lowerUserInput.Equals("cash-register") ||
                lowerUserInput.Equals("cash register") ||
                lowerUserInput.Equals("register") ||
                lowerUserInput.Equals("status");
        }

        private void PrintCashRegisterStatus()
        {
            IEnumerable<CashRegister> cashRegisters = mFinanceDiaryManager.GetAllCashRegisters();

            string[] columnsHeaders = cashRegisters.Select(cash => cash.Name).Append("Total").ToArray();
            ConsoleTable consoleTable = new ConsoleTable(columnsHeaders);

            int[] amounts = cashRegisters.Select(cash => cash.CurrentAmount).ToArray();
            object[] amountsObject = new object[columnsHeaders.Length];

            int total = 0;
            for(int i = 0; i < amounts.Length; ++i)
            {
                total += amounts[i];
                amountsObject[i] = amounts[i];
            }

            amountsObject[columnsHeaders.Length - 1] = total;

            consoleTable.AddRow(amountsObject);

            consoleTable.Write(Format.Alternative);
        }

        private int Save(CommandLineOptions.SaveOptions _)
        {
            mFinanceDiaryManager.SaveToDatabase();

            return 0;
        }
    }
}