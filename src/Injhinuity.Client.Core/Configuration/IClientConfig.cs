namespace Injhinuity.Client.Core.Configuration
{
    public interface IClientConfig
    {
        VersionConfig Version { get; }
        LoggingConfig Logging { get; }
        DiscordConfig Discord { get; }
        ApiConfig Api { get; }
        ValidationConfig Validation { get; }
    }
}
