
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;

namespace Tester.Services
{
    //not used in the code yet
    public interface IApiCallerFactory
    {
        ICallApi Build();
    }

    public class ApiCallerFactory : IApiCallerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _serviceProvider;

        public ApiCallerFactory(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
        }

        public ICallApi Build()
        {
            using var scope = _serviceProvider.CreateScope();
            var request = _serviceProvider.GetRequiredService<IRequest>();
            var response = _serviceProvider.GetRequiredService<IResponse>();
            var console = _serviceProvider.GetRequiredService<IConsoleWrapper>();
            return new ApiClient(_loggerFactory.CreateLogger<ApiClient>(), request, response, console);
        }
    }
}
