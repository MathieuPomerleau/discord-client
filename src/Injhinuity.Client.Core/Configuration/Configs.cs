using Microsoft.Extensions.Logging;

namespace Injhinuity.Client.Core.Configuration
{
    public record ApiConfig(string BaseUrl) {}
    public record ClientConfig(VersionConfig Version, LoggingConfig Logging, DiscordConfig Discord, ApiConfig Api, ValidationConfig Validation) : IClientConfig {}
    public record CommandValidationConfig(long NameMaxLength, long BodyMaxLength) {}
    public record DiscordConfig(string Token, char Prefix) {}
    public record LoggingConfig(LogLevel LogLevel) {}
    public record ValidationConfig(CommandValidationConfig Command) {}
    public record VersionConfig(string VersionNo) {}
}
