using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Model.Domain;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Services
{
    public class ReactionRoleEmbedServiceTests
    {
        private static readonly IFixture Fixture = new Fixture();

        private readonly IReactionRoleEmbedService _subject;

        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly IRoleEmbedBuilderFactory _roleEmbedBuilderFactory;
        private readonly IInjhinuityCommandContext _context;
        private readonly IReactionRoleEmbed _reactionRoleEmbed;
        private readonly IUserMessage _userMessage;
        private readonly IGuild _guild;

        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private readonly IEnumerable<Role> _roles = Fixture.CreateMany<Role>();

        public ReactionRoleEmbedServiceTests()
        {
            _reactionEmbedFactory = Substitute.For<IReactionEmbedFactory>();
            _embedBuilderFactoryProvider = Substitute.For<IEmbedBuilderFactoryProvider>();
            _roleEmbedBuilderFactory = Substitute.For<IRoleEmbedBuilderFactory>();
            _context = Substitute.For<IInjhinuityCommandContext>();
            _reactionRoleEmbed = Substitute.For<IReactionRoleEmbed>();
            _userMessage = Substitute.For<IUserMessage>();
            _guild = Substitute.For<IGuild>();

            _embedBuilderFactoryProvider.Role.Returns(_roleEmbedBuilderFactory);
            _roleEmbedBuilderFactory.CreateReactionRole().Returns(_embedBuilder);
            _reactionEmbedFactory.CreateRoleReactionEmbed(default, default, default).ReturnsForAnyArgs(_reactionRoleEmbed);

            _subject = new ReactionRoleEmbedService(_reactionEmbedFactory, _embedBuilderFactoryProvider);
        }

        [Fact]
        public async Task InitializeAsync_ThenExecutesProperly()
        {
            await _subject.InitializeAsync(_context, _roles);

            await _reactionRoleEmbed.Received().InitalizeAsync(_context);
        }

        [Fact]
        public async Task InitializeFromMessageAsync()
        {
            await _subject.InitializeFromMessageAsync(_guild, _userMessage, _roles);

            await _reactionRoleEmbed.Received().InitalizeFromExistingAsync(_userMessage);
        }

        [Fact]
        public async Task ResetAsync_WithNullReactionRoleEmbed_ThenDoesntResetReactionRoleEmbed()
        {
            await _subject.ResetAsync();

            await _reactionRoleEmbed.DidNotReceive().ResetAsync();
        }

        [Fact]
        public async Task ResetAsync_WithNullReactionRoleEmbed()
        {
            await _subject.InitializeFromMessageAsync(_guild, _userMessage, _roles);

            await _subject.ResetAsync();

            await _reactionRoleEmbed.Received().ResetAsync();
        }
    }
}
