using System.Net;

namespace Injhinuity.Client.Core.Exceptions
{
    public record ExceptionWrapper(HttpStatusCode StatusCode, string? Name = null, string? Message = null, string? Reason = null) {}
}
