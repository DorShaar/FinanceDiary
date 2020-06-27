using CommandLine;
using FinanceDiary.App;
using FinanceDiary.Domain.FinanceOperations;
using Microsoft.Extensions.Logging;
using System;

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
                CommandLineOptions.GetOptions>(args).MapResult(
                    (CommandLineOptions.DepositOptions options) => Deposit(options),
                    (parserErrors) => 1
                );
        }

        private int Deposit(CommandLineOptions.DepositOptions options)
        {
            if (!mFinanceDiaryManager.AddFinanceOperation(
                options.Date,
                OperationType.Deposit,
                options.Amount,
                (OperationKind)options.Kind,
                options.Reason))
            {
                mLogger.LogWarning($"Deposit operation of {options.Amount} at {options.Date} of kind {options.Kind} " +
                    $"and reason {options.Reason} failed");
                return 1;
            }

            return 0;
        }
    }
}