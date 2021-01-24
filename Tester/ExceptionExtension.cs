using Flurl.Http;

using Microsoft.Extensions.Logging;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tester.Services
{

    public static class ExceptionExtension
    {
        public static string Deflate(this Exception ex)
        {
            return $"Type:{ex.GetType()}^ Message:{ex.Message}^ Stack{ex.StackTrace}";
        }
    }
}
