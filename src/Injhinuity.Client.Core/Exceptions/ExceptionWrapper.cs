using System.Net;

namespace Injhinuity.Client.Core.Exceptions
{
    public class ExceptionWrapper
    {
        public string? Name { get; set; }
        public string? Message { get; set; }
        public string? Reason { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
