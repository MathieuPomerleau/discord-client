namespace Injhinuity.Client.Core.Configuration.Options
{
    public interface IClientOptions : INullableOption
    {
        static string SectionName => "Client";

        VersionOptions? Version { get; set; }
        LoggingOptions? Logging { get; set; }
        DiscordOptions? Discord { get; set; }
    }

    public class ClientOptions : IClientOptions
    {
        private const string OptionName = "Client";

        public VersionOptions? Version { get; set; }
        public LoggingOptions? Logging { get; set; }
        public DiscordOptions? Discord { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (Version is null)
                result.AddValueToResult(OptionName, "Version");
            else
                Version.ContainsNull(result);

            if (Logging is null)
                result.AddValueToResult(OptionName, "Logging:");
            else
                Logging.ContainsNull(result);

            if (Discord is null)
                result.AddValueToResult(OptionName, "Discord");
            else
                Discord.ContainsNull(result);
        }
    }
}
