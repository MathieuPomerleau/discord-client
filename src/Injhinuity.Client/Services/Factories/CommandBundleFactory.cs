﻿using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Model.Domain.Requests.Bundles;

namespace Injhinuity.Client.Services.Factories
{
    public interface ICommandBundleFactory
    {
        CommandRequestBundle Create(string guildId);
        CommandRequestBundle Create(string guildId, string name);
        CommandRequestBundle Create(string guildId, string name, string body);
    }

    public class CommandBundleFactory : ICommandBundleFactory
    {
        public CommandRequestBundle Create(string guildId) =>
            new CommandRequestBundle(guildId);

        public CommandRequestBundle Create(string guildId, string name) =>
            Create(guildId, name, "");

        public CommandRequestBundle Create(string guildId, string name, string body) =>
            new CommandRequestBundle(guildId, new CommandRequest(name, body));
    }
}
