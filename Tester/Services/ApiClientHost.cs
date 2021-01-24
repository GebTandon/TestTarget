using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tester.Services
{
    //https://dotnetcorecentral.com/blog/background-tasks/
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio
    internal class ApiClientHost : BackgroundService, IDisposable
    {
        private bool disposedValue;
        private readonly IServiceProvider _services;
        private readonly ILogger<ApiClientHost> _logger;
        private readonly AppSettings _settings;
        private readonly IConsoleWrapper _consoleWrapper;

        public ApiClientHost(IServiceProvider services, ILogger<ApiClientHost> logger, IOptions<AppSettings> settings, IConsoleWrapper consoleWrapper)
        {
            _services = services;
            _logger = logger;
            _settings = settings.Value;
            _consoleWrapper = consoleWrapper;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ApiClientHost()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion Dispose

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting the {nameof(ApiClientHost)}.");
            await LoadServer(stoppingToken).ConfigureAwait(false);
        }

        private async Task LoadServer(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var loader = scope.ServiceProvider.GetRequiredService<ILoadServer>();
            _consoleWrapper.Write("Loading the server...");
            await loader.CallServer(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping the {nameof(ApiClientHost)}.");
            return base.StopAsync(cancellationToken);
        }
    }
}