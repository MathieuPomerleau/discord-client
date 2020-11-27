using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Extensions;
using Injhinuity.Client.Model.Domain;

namespace Injhinuity.Client.Discord.Services
{
    public interface IReactionRoleEmbedService
    {
        Task<ulong> InitializeAsync(IInjhinuityCommandContext context, IEnumerable<Role> roles);
        Task InitializeFromMessageAsync(IGuild guild, IUserMessage message, IEnumerable<Role> roles);
        Task ResetAsync();
    }

    public class ReactionRoleEmbedService : IReactionRoleEmbedService
    {
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;

        private IReactionRoleEmbed? _current;

        public ReactionRoleEmbedService(IReactionEmbedFactory reactionEmbedFactory, IEmbedBuilderFactoryProvider embedBuilderFactoryProvider)
        {
            _reactionEmbedFactory = reactionEmbedFactory;
            _embedBuilderFactoryProvider = embedBuilderFactoryProvider;
        }

        public async Task<ulong> InitializeAsync(IInjhinuityCommandContext context, IEnumerable<Role> roles)
        {
            (var rows, var buttons) = CreateRowsAndButtons(context.Guild, roles);

            var embedBuilder = _embedBuilderFactoryProvider.Role.CreateReactionRole();
            _current = _reactionEmbedFactory.CreateRoleReactionEmbed(embedBuilder, rows, buttons);
            return await _current.InitalizeAsync(context);
        }

        public async Task ResetAsync()
        {
            if (_current is not null)
                await _current.ResetAsync();

            _current = null;
        }

        public async Task InitializeFromMessageAsync(IGuild guild, IUserMessage message, IEnumerable<Role> roles)
        {
            (var rows, var buttons) = CreateRowsAndButtons(guild, roles);

            var embedBuilder = _embedBuilderFactoryProvider.Role.CreateReactionRole();
            _current = _reactionEmbedFactory.CreateRoleReactionEmbed(embedBuilder, rows, buttons);
            await _current.InitalizeFromExistingAsync(message);
        }

        private RowButtonContainer CreateRowsAndButtons(IGuild guild, IEnumerable<Role> roles)
        {
            var rows = roles.Select(x => $"{x.EmoteString} => {x.Name}");
            var buttons = roles.Select(x => new UserReactionButton(Emote.Parse(x.EmoteString), async (userId, channel) =>
            {
                var user = await guild.GetUserAsync(userId);
                var role = guild.GetRole(ulong.Parse(x.Id));

                if (user.RoleIds.ToArray().Contains(role.Id))
                {
                    await user.RemoveRoleAsync(role);
                    var embedBuilder = _embedBuilderFactoryProvider.Role.CreateUnassignRoleSuccess(user.Username, role.Name);
                    await channel.SendEmbedMessageAsync(embedBuilder);
                }
                else
                {
                    await user.AddRoleAsync(role);
                    var embedBuilder = _embedBuilderFactoryProvider.Role.CreateAssignRoleSuccess(user.Username, role.Name);
                    await channel.SendEmbedMessageAsync(embedBuilder);
                }
            }));

            return new RowButtonContainer { Rows = rows, Buttons = buttons };
        }

        private struct RowButtonContainer
        {
            public IEnumerable<string> Rows { get; set; }
            public IEnumerable<UserReactionButton> Buttons { get; set; }

            public void Deconstruct(out IEnumerable<string> rows, out IEnumerable<UserReactionButton> buttons)
            {
                rows = Rows;
                buttons = Buttons;
            }
        }
    }
}
