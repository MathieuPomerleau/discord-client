using Microsoft.Extensions.Logging;

namespace Injhinuity.Client.Core.Configuration.Options
{
    public class LoggingOptions : INullableOption
    {
        public LogLevel? LogLevel { get; set; }

        public bool ContainsNull() =>
            LogLevel is null;
    }
}
