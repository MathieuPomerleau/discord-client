namespace Injhinuity.Client.Configuration
{
    public interface IClientConfig
    {
        VersionConfig? Version { get; set; }
        LoggingConfig? Logging { get; set; }
    }

    public class ClientConfig : IClientConfig
    {
        public VersionConfig? Version { get; set; }
        public LoggingConfig? Logging { get; set; }
    }
}
