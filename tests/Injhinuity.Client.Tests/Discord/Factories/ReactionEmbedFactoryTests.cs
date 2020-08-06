using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Content;
using Injhinuity.Client.Discord.Emotes;
using Injhinuity.Client.Discord.Factories;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Factories
{
    public class ReactionEmbedFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IReactionEmbedFactory _subject;

        private readonly IEnumerable<InjhinuityEmbedField> _fields = Fixture.CreateMany<InjhinuityEmbedField>();
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();

        private readonly IReactionEmbedBuilder _builder;

        public ReactionEmbedFactoryTests()
        {
            _builder = Substitute.For<IReactionEmbedBuilder>();
            _builder.ReturnsForAll(_builder);

            _subject = new ReactionEmbedFactory(_builder);
        }

        [Fact]
        public void CreateListReactionEmbed_WhenCalled_ProperlyCreatesAReactionEmbed()
        {
            var result = _subject.CreateListReactionEmbed(_fields, _embedBuilder);

            Received.InOrder(() =>
            {
                _builder.Create();
                _builder.WithLifetime(60);
                _builder.WithContent(Arg.Any<ListEmbedContent>());
                _builder.WithButton(InjhinuityEmote.LeftArrow, Arg.Any<Func<Task>>());
                _builder.WithButton(InjhinuityEmote.RightArrow, Arg.Any<Func<Task>>());
                _builder.Build();
            });
        }
    }
}
