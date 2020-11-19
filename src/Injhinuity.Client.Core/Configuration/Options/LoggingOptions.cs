using Microsoft.Extensions.Logging;

namespace Injhinuity.Client.Core.Configuration.Options
{
    public class LoggingOptions : INullableOption
    {
        public static string OptionName => "Logging";

        public LogLevel? AppLogLevel { get; set; }
        public LogLevel? DiscordLogLevel { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (AppLogLevel is null)
                result.AddValueToResult(OptionName, "AppLogLevel");

            if (DiscordLogLevel is null)
                result.AddValueToResult(OptionName, "DiscordLogLevel");
        }
    }
}
