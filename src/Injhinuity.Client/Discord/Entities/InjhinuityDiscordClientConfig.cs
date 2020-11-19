using Discord.WebSocket;

namespace Injhinuity.Client.Discord.Entities
{
    public interface IInjhinuityDiscordClientConfig
    {
        DiscordSocketConfig Config { get; init; }
    }

    public record InjhinuityDiscordClientConfig(DiscordSocketConfig Config) : IInjhinuityDiscordClientConfig {}
}
