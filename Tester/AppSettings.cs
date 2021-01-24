using System;
using System.Net.Http;

namespace Tester
{
    public class AppSettings
    {
        HttpMethod Method = null;
        Type Customizer = null;
        public string Url { get; set; }
        public string Verb { get; set; }
        public string RequestFile { get; set; }
        public string RequestCustomizer { get; set; }
        public int BaseLoad { get; set; }

        public HttpMethod GetVerb()
        {
            return Method ??= new HttpMethod(Verb);
        }

        public Type GetCustomizerType()
        {
            return Customizer ??= Type.GetType(RequestCustomizer, true, true);
        }
    }
}