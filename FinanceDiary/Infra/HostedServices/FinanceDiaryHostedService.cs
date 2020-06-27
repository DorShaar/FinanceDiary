using ConsoleTables;
using FinanceDiary.Infra.ClientApi;
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
        private readonly ILogger<FinanceDiaryHostedService> mLogger;

        public FinanceDiaryHostedService(ICommandLineRunner commandLineRunner, ILogger<FinanceDiaryHostedService> logger)
        {
            mCommandLineRunner = commandLineRunner;
            mLogger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mLogger.LogInformation($"Started hosted service Finance-Diary-App");

            string userInput = Console.ReadLine();

            while (!ShouldExit(userInput))
            {
                mCommandLineRunner.RunCommand(userInput.Split(" "));
                //Print();
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

        public void Print()
        {
            ConsoleTable consoleTable = new ConsoleTable("1", "2", "3");
            consoleTable.AddRow("abc", "cfg", "dfdf");
            consoleTable.AddRow("ab555c", "cf3434g", "dfd2131231");

            consoleTable.Write(Format.Alternative);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mLogger.LogInformation($"Stopped hosted service Finance-Diary-App");

            // TODO Save here.

            return Task.CompletedTask;
        }
    }
}