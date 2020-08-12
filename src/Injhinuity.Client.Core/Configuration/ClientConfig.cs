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

    public class ClientConfig : IClientConfig
    {
        public VersionConfig Version { get; }
        public LoggingConfig Logging { get; }
        public DiscordConfig Discord { get; }
        public ApiConfig Api { get; }
        public ValidationConfig Validation { get; }

        public ClientConfig(VersionConfig version, LoggingConfig logging, DiscordConfig discord, ApiConfig api, ValidationConfig validation)
        {
            Version = version;
            Logging = logging;
            Discord = discord;
            Api = api;
            Validation = validation;
        }
    }
}
