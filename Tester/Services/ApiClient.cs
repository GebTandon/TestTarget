using Flurl.Http;

using Microsoft.Extensions.Logging;

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

        public ApiClient(ILogger<ApiClient> logger, IRequest request, IResponse response, IConsoleWrapper consoleWrapper)
        {
            _logger = logger;
            _request = request;
            _response = response;
            _consoleWrapper = consoleWrapper;
        }

        public async Task CallApiAsync(CancellationToken cancel)
        {
            try
            {
                if (_request.Verb == HttpMethod.Get)
                    _response.Value = await _request.GetUrl()
                        .GetStringAsync(cancel)
                        .ConfigureAwait(false);
                else
                    _response.Value = await _request.GetUrl().SendJsonAsync(_request.Verb, _request.GetValue(), cancel).ReceiveJson();
            }
            catch (Exception ex)
            {
                _consoleWrapper.Write($"Exception executing {_request.Number} request#. {ex.Deflate()}",ConsoleColor.Red);
                _response.Error = ex;
            }
            _response.Number = _request.Number;
        }
    }
}
