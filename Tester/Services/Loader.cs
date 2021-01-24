using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tester.Services
{
    public interface ILoadServer
    {
        Task CallServer(CancellationToken cancel);
        long ClientCount(long newCount);
    }

    public class Loader : ILoadServer, IDisposable
    {
        private readonly ILogger<Loader> _logger;
        private readonly AppSettings _settings;
        private readonly IApiCallerFactory _callerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsoleWrapper _consoleWrapper;
        private long MaxClient;
        private bool disposedValue;
        private List<Task> _tasks;

        public Loader(ILogger<Loader> logger,IOptions<AppSettings> settings,  IApiCallerFactory callerFactory, IServiceProvider serviceProvider, IConsoleWrapper consoleWrapper)
        {
            MaxClient = 1;//at least 1
            _logger = logger;
            _settings = settings.Value;
            _callerFactory = callerFactory;
            _serviceProvider = serviceProvider;
            _consoleWrapper = consoleWrapper;
            MaxClient = _settings.BaseLoad;
            _tasks = new List<Task>();
        }

        public async Task CallServer(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                await BombardServer(cancel);
            }
        }

        public long ClientCount(long newCount)
        {
            var oldValue = Interlocked.Exchange(ref MaxClient, newCount);
            return oldValue;
        }

        private async Task BombardServer(CancellationToken cancel)
        {
            var scope = _serviceProvider.CreateScope();
            var tasksCount = MaxClient;
            _consoleWrapper.Write($"Starting a new round of {tasksCount} clients..",ConsoleColor.Green);
            _tasks.Clear();
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < MaxClient; i++)
            {
                //var request = _serviceProvider.GetRequiredService<IRequest>();
                //var response = _serviceProvider.GetRequiredService<IResponse>();
                var apiClient = scope.ServiceProvider.GetRequiredService<ICallApi>();
                _tasks.Add(apiClient.CallApiAsync(cancel));
            }
            await Task.WhenAll(_tasks.ToArray());
            _consoleWrapper.Write($"Finished round of {tasksCount} in {stopwatch.Elapsed}", ConsoleColor.Green);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _logger.LogInformation("Terminating Loader ...");
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Loader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
