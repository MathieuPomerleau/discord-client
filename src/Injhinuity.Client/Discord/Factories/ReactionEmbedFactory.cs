using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Content;
using Injhinuity.Client.Discord.Emotes;
using InjhinuityEmbedField = Injhinuity.Client.Discord.Embeds.InjhinuityEmbedField;

namespace Injhinuity.Client.Discord.Factories
{
    public interface IReactionEmbedFactory
    {
        IPageReactionEmbed CreatePageReactionEmbed(EmbedBuilder embedBuilder, IEnumerable<InjhinuityEmbedField>? fields);
        IReactionRoleEmbed CreateRoleReactionEmbed(EmbedBuilder embedBuilder, IEnumerable<string> rows, IEnumerable<UserReactionButton> buttons);
    }

    public class ReactionEmbedFactory : IReactionEmbedFactory
    {
        private readonly IReactionEmbedBuilder _builder;

        public ReactionEmbedFactory(IReactionEmbedBuilder builder)
        {
            _builder = builder;
        }

        public IPageReactionEmbed CreatePageReactionEmbed(EmbedBuilder embedBuilder, IEnumerable<InjhinuityEmbedField>? fields)
        {
            var content = new PagedListEmbedContent(9, fields, embedBuilder);

            return _builder.Create()
                .WithLifetime(60)
                .WithContent(content)
                .WithButton(InjhinuityEmote.LeftArrow, () =>
                {
                    content.PreviousPage();
                    return Task.CompletedTask;
                })
                .WithButton(InjhinuityEmote.RightArrow, () =>
                {
                    content.NextPage();
                    return Task.CompletedTask;
                })
                .BuildPage();
        }

        public IReactionRoleEmbed CreateRoleReactionEmbed(EmbedBuilder embedBuilder, IEnumerable<string> rows, IEnumerable<UserReactionButton> buttons)
        {
            var content = new DescriptionListEmbedContent(embedBuilder, rows);

            return _builder.Create()
                .WithContent(content)
                .WithUserButtons(buttons)
                .BuildRole();
        }
    }
}
