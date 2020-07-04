using FinanceDiary.Infra.ClientApi;
using FinanceDiary.Infra.CommandParsers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceDiary.Infra.HostedServices
{
    public class FinanceDiaryHostedService : IHostedService
    {
        private readonly ICommandLineRunner mCommandLineRunner;
        private readonly ICommandParser mCommandParser;
        private readonly ILogger<FinanceDiaryHostedService> mLogger;

        public FinanceDiaryHostedService(ICommandLineRunner commandLineRunner,
            ICommandParser commandParser,
            ILogger<FinanceDiaryHostedService> logger)
        {
            mCommandLineRunner = commandLineRunner;
            mCommandParser = commandParser;
            mLogger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mLogger.LogInformation($"Started hosted service Finance-Diary-App");

            string userInput = Console.ReadLine();

            while (!ShouldExit(userInput))
            {
                mCommandLineRunner.RunCommand(mCommandParser.Parse(userInput));
                userInput = Console.ReadLine();
            }
                
            return Task.CompletedTask;
        }

        private bool ShouldExit(string userInput)
        {
            return
                userInput.ToLowerInvariant().Equals("exit") ||
                userInput.ToLowerInvariant().Equals("stop") ||
                userInput.ToLowerInvariant().Equals("x") ||
                userInput.ToLowerInvariant().Equals("bye");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mLogger.LogInformation($"Stopped hosted service Finance-Diary-App");

            return Task.CompletedTask;
        }
    }
}