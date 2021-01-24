
using System;

namespace Tester.Services
{

    public interface IResponse
    {
        object Value { get; set; }
        Exception Error { get; set; }
        long Number { get; set; }
    }
    public class ResponseObj : IResponse
    {
        public object Value { get; set; }
        public Exception Error { get; set; }
        public long Number { get; set; }
    }
}
