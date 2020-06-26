using FinanceDiary.Infra.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceDiary.Infra
{
    public class FinanceDiaryMicroserviceHost
    {
        private readonly Action<IServiceCollection> mConfigTestServices;
        private ILogger<FinanceDiaryMicroserviceHost> mLogger;

        public FinanceDiaryMicroserviceHost(Action<IServiceCollection> configTestServices = null)
        {
            mConfigTestServices = configTestServices;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken)
        {
            try
            {
                using IHost host = CreateHost(args);
                mLogger = host.Services.GetRequiredService<ILogger<FinanceDiaryMicroserviceHost>>();
                mLogger.LogInformation("Starting FinacneDiary microservice");
                await host.RunAsync(cancellationToken).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                mLogger.LogError(e, "FinacneDiary service failed to start");
                Environment.ExitCode = -1;
            }

            return Environment.ExitCode;
        }

        private void CurrentDomain_UnhandledException(object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            if (!(unhandledExceptionEventArgs.ExceptionObject is Exception unhandledException))
                return;

            mLogger.LogError(unhandledException, $"Unhandled exception occured");
        }

        private IHost CreateHost(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     if (hostingContext.HostingEnvironment.IsProduction())
                     {
                         config.AddYamlFile(Path.Combine("Infra", "Options", "FinanceDiaryConfig.yaml"), optional: false);
                     }
                 })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddFinanceDiaryDependencies(hostBuilderContext.Configuration);

                    services.Configure<HostOptions>(option => option.ShutdownTimeout = TimeSpan.FromSeconds(10));

                    mConfigTestServices?.Invoke(services);
                })
                .UseSerilog((hostBuilderContext, loggerConfig) =>
                {
                    loggerConfig.ReadFrom.Configuration(hostBuilderContext.Configuration)
                        .Enrich.WithMachineName();
                }).Build();
        }
    }
}