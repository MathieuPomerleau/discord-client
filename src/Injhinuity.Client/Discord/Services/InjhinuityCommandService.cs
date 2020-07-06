using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Injhinuity.Client.Discord.Services
{
    public interface IInjhinuityCommandService
    {
        Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider provider);
        Task<IResult> ExecuteAsync(ICommandContext context, int argPos, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception);
        event Func<LogMessage, Task> Log;
        event Func<Optional<CommandInfo>, ICommandContext, IResult, Task> CommandExecuted;
    }

    public class InjhinuityCommandService : CommandService, IInjhinuityCommandService { }
}
