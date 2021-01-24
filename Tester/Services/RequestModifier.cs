using System.Text.Json;

namespace Tester.Services
{
    //This class can be put into a different library and loaded through DI or reflection to allow request customization..
    public class RequestModifier : IRequestUpdater
    {
        public object Customize(object request)
        {
            var reqst = JsonSerializer.Deserialize<SampleRequest>(request as string);
            reqst.Id += 1;//customize the request, before we post to server.
            return reqst; //return the deserialized object
        }
    }

    public class SampleRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
