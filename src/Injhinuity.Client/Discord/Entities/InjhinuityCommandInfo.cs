using Discord;
using Discord.Commands;

namespace Injhinuity.Client.Discord.Entities
{
    public interface IInjhinuityCommandInfo
    {
        Optional<CommandInfo> CommandInfo { get; }
    }

    public class InjhinuityCommandInfo : IInjhinuityCommandInfo
    {
        public Optional<CommandInfo> CommandInfo { get; }

        public InjhinuityCommandInfo(Optional<CommandInfo> commandInfo)
        {
            CommandInfo = commandInfo;
        }
    }
}
