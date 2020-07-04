using CommandLine;
using ConsoleTables;
using FinanceDiary.App;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.ConsoleTableAdapters;
using FinanceDiary.Domain.FinanceOperations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
            catch (Exception ex)
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

                if (IsReferringReport(options.ObjectType))
                {
                    PrintReportSince(options.Since);
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
                lowerUserInput.Equals("registers") ||
                lowerUserInput.Equals("status");
        }

        private void PrintCashRegisterStatus()
        {
            IEnumerable<CashRegister> cashRegisters = mFinanceDiaryManager.GetAllCashRegisters();

            ConsoleTableAdapter.PrintCashRegisterStatus(cashRegisters);
        }

        private bool IsReferringReport(string userInput)
        {
            string lowerUserInput = userInput.ToLowerInvariant();
            return
                lowerUserInput.Equals("report") ||
                lowerUserInput.Equals("reports");
        }

        private void PrintReportSince(string since)
        {
            ConsoleTableAdapter.PrintReportSince(mFinanceDiaryManager.GetReport(), CalculateDateTimeSince(since));
        }

        private DateTime CalculateDateTimeSince(string since)
        {
            if (string.IsNullOrEmpty(since))
                return default;

            int timeNumber;

            char firstLetter = since.FirstOrDefault(ch => char.IsLetter(ch));

            if (firstLetter == default)
            {
                timeNumber = int.Parse(since);
                return DateTime.Now.AddMonths(-timeNumber);
            }

            int firstLetterIndex = since.IndexOf(firstLetter);
            string timeNumberStr = since.Substring(0, firstLetterIndex);
            timeNumber = int.Parse(timeNumberStr);

            char timeType = since[firstLetterIndex];

            if (timeType.Equals('m'))
                return DateTime.Now.AddMonths(-timeNumber);
            if (timeType.Equals('y'))
                return DateTime.Now.AddYears(-timeNumber);

            throw new ArgumentException($"Could not parse paramter {nameof(since)}: {since}");
        }

        private int Save(CommandLineOptions.SaveOptions _)
        {
            mFinanceDiaryManager.SaveToDatabase().Wait();

            return 0;
        }
    }
}