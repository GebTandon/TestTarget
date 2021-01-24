using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Tester.Services;

namespace Tester
{
    //can run this app with command line args eg. $> dotnet run Tester.csproj /AppSettings:BaseLoad=15
    //https://stackoverflow.com/questions/671163/can-you-have-multiple-net-consoles-as-in-console-writeline
    //https://xspdf.com/resolution/58277690.html#:~:text=The%20Environment%20object%20has%20a,application%20with%20some%20exit%20code.
    public static class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args);
            var host = hostBuilder.Build();
            var services = host.Services;
            var cancellationSource = new CancellationTokenSource();
            var consoleSrv = services.GetRequiredService<IConsoleWrapper>();
            var loaderSrv = services.GetRequiredService<ILoadServer>();
            _ = PromptUser(consoleSrv, loaderSrv, cancellationSource);//do not await on this task !!
            await host.RunAsync(cancellationSource.Token);
            cancellationSource.Dispose();
        }

        private static Task PromptUser(IConsoleWrapper consoleSrv, ILoadServer loaderSrv, System.Threading.CancellationTokenSource cancellationSource)
        {
            return Task.Run(() =>
            {
                do
                {
                    var loadCount = consoleSrv.PromptAndRead("Provide load count...");
                    if (long.TryParse(loadCount, out var count))
                        loaderSrv.ClientCount(count);
                    if (loadCount.Equals("c", StringComparison.InvariantCultureIgnoreCase) || loadCount.Equals("x", StringComparison.InvariantCultureIgnoreCase))
                    {
                        cancellationSource.Cancel();
                        //Environment.Exit(0);
                    }
                } while (true);
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseEnvironment("Development")
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureHostConfiguration(cfgBldr =>
            {
                cfgBldr.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hostsettings.json", optional: true)
                .AddEnvironmentVariables(prefix: "PREFIX_")
                ;
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var ctxdir = AppContext.BaseDirectory;
                var env = hostingContext.HostingEnvironment.EnvironmentName;
                config.Sources.Clear();//clear all the providers set through default configurations
                config.SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables("CSL_")
                .AddJsonFile("appsettings.json", false, true) //Reload on change set to true so that IOptionsSnapshot and IOptionsMonitor work well.
                .AddJsonFile($"appsettings.{env}.json", true, false)
                .AddCommandLine(args)
                ;
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions()
                    .Configure<HostOptions>(bndrOpt =>
                    {
                        //additional host options can be setup here, currently only ShutdownTimeout !
                        //bndrOpt.ShutdownTimeout = new TimeSpan(0, 1, 0, 20, 0);
                    })
                    .AddHostedService<ApiClientHost>()
                    .AddSingleton<ILoadServer, Loader>()
                    .AddSingleton<IApiCallerFactory, ApiCallerFactory>()
                    .AddSingleton<IConsoleWrapper, WrappedConsole>()
                    .AddScoped<ICallApi, ApiClient>()
                    .AddScoped<IRequest, RequestObj>()
                    .AddScoped<IResponse, ResponseObj>()
                    .Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings")) //for allowing IOptions or IOptionsSnapshot with default config to work.
                    ;
            });
    }
}
