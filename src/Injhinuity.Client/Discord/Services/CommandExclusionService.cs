using System.Collections.Generic;

namespace Injhinuity.Client.Discord.Services
{
    public interface ICommandExclusionService
    {
        bool IsExcluded(string command);
    }

    public class CommandExclusionService : ICommandExclusionService
    {
        private readonly IReadOnlySet<string> _excludedCommands = new HashSet<string> {
            "info"
        };

        public bool IsExcluded(string command) => _excludedCommands.Contains(command);
    }
}
