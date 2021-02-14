using Flurl.Http;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tester.Services
{
    public interface ICallApi
    {
        Task CallApiAsync(CancellationToken cancel);
    }


    public class ApiClient : ICallApi
    {
        private readonly ILogger<ApiClient> _logger;
        private readonly IRequest _request;
        private readonly IResponse _response;
        private readonly IConsoleWrapper _consoleWrapper;
        private readonly AsyncRetryPolicy _policy;

        public ApiClient(ILogger<ApiClient> logger, IRequest request, IResponse response, IConsoleWrapper consoleWrapper)
        {
            _logger = logger;
            _request = request;
            _response = response;
            _consoleWrapper = consoleWrapper;
            _policy = Policy
                .Handle<FlurlHttpException>(ex => (ex.StatusCode ?? 100) > 500)
                .Or<FlurlHttpTimeoutException>()
                .WaitAndRetryAsync(3, (ctr) => TimeSpan.FromMilliseconds(100 * ctr));
        }

        public Task CallApiAsync(CancellationToken cancel)
        {
            return _policy.ExecuteAsync(() => CallApiAsyncInt(cancel));
        }
        private async Task CallApiAsyncInt(CancellationToken cancel)
        {
            try
            {
                var urlToCall = _request.GetUrl();
                _response.Value = _request.Verb == HttpMethod.Get
                    ? await urlToCall
                        .GetStringAsync(cancel)
                        .ConfigureAwait(false)
                    : (object)await urlToCall
                        .SendJsonAsync(_request.Verb, _request.GetValue(), cancel)
                        .ReceiveJson();
            }
            catch (Exception ex)
            {
                _consoleWrapper.Write($"Exception executing {_request.Number} request#. {ex.Deflate()}", ConsoleColor.Red);
                _response.Error = ex;
            }
            _response.Number = _request.Number;
        }
    }
}
