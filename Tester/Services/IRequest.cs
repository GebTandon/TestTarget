using Microsoft.Extensions.Options;

using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Tester.Services
{

    public interface IRequest
    {
        object GetValue();
        string GetUrl();
        HttpMethod Verb { get; set; }
        long Number { get; }
    }

    public class RequestObj : IRequest
    {
        protected static long Counter;

        public long Number { get; protected set; }
        private readonly AppSettings _settings;
        private readonly string _url;

        public HttpMethod Verb { get; set; }

        public RequestObj(IOptions<AppSettings> settings)
        {
            Number = Interlocked.Increment(ref Counter);
            _settings = settings.Value;

            _url = _settings.Url;
            Verb = _settings.GetVerb();
        }

        public object GetValue()
        {
            if (Verb == HttpMethod.Get)
                return null;
            var jsonRequest = File.ReadAllText(_settings.RequestFile);
            var customizer = Activator.CreateInstance(_settings.GetCustomizerType()) as IRequestUpdater;
            return customizer.Customize(jsonRequest);
        }

        public string GetUrl()
        {
            return _url;
        }

    }

    internal interface IRequestUpdater
    {
        object Customize(object request);
    }
}
