using Microsoft.Extensions.Logging;

namespace Injhinuity.Client.Core.Configuration.Options
{
    public class LoggingOptions : INullableOption
    {
        public static string OptionName => "Logging";

        public LogLevel? LogLevel { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (LogLevel is null)
                result.AddValueToResult(OptionName, "LogLevel");
        }
    }
}
