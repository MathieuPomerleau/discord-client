namespace Injhinuity.Client.Core.Configuration.Options
{
    public interface IClientOptions : INullableOption
    {
        static string SectionName => "Client";

        VersionOptions? Version { get; set; }
        LoggingOptions? Logging { get; set; }
        DiscordOptions? Discord { get; set; }
        ApiOptions? Api { get; set; }
        ValidationOptions? Validation { get; set; }
    }

    public class ClientOptions : IClientOptions
    {
        public static string OptionName => "Client";

        public VersionOptions? Version { get; set; }
        public LoggingOptions? Logging { get; set; }
        public DiscordOptions? Discord { get; set; }
        public ApiOptions? Api { get; set; }
        public ValidationOptions? Validation { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (Version is null)
                result.AddValueToResult(OptionName, VersionOptions.OptionName);
            else
                Version.ContainsNull(result);

            if (Logging is null)
                result.AddValueToResult(OptionName, LoggingOptions.OptionName);
            else
                Logging.ContainsNull(result);

            if (Discord is null)
                result.AddValueToResult(OptionName, DiscordOptions.OptionName);
            else
                Discord.ContainsNull(result);

            if (Api is null)
                result.AddValueToResult(OptionName, ApiOptions.OptionName);
            else
                Api.ContainsNull(result);

            if (Validation is null)
                result.AddValueToResult(OptionName, ValidationOptions.OptionName);
            else
                Validation.ContainsNull(result);
        }
    }
}
