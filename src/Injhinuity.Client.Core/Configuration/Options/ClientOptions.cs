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
        public static string SectionName => "Client";

        public VersionOptions? Version { get; set; }
        public LoggingOptions? Logging { get; set; }
        public DiscordOptions? Discord { get; set; }

        public bool ContainsNull() =>
            (Version?.ContainsNull() ?? true) ||
            (Logging?.ContainsNull() ?? true) ||
            (Discord?.ContainsNull() ?? true);
    }
}
