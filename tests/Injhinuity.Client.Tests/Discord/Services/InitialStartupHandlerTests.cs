using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Services
{
    public class InitialStartupHandlerTests
    {
        private static readonly IFixture Fixture = new Fixture();

        private readonly IInitialStartupHandler _subject;

        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IReactionRoleEmbedService _reactionRoleEmbedService;
        private readonly IGuildRequester _guildRequester;
        private readonly IGuildBundleFactory _guildBundleFactory;
        private readonly IRoleRequester _roleRequester;
        private readonly IRoleBundleFactory _roleBundleFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly ITextChannel _textChannel;
        private readonly IUserMessage _userMessage;
        private readonly IGuild _guild;

        private readonly ulong _channelId = Fixture.Create<ulong>();
        private readonly ulong _messageId = Fixture.Create<ulong>();
        private readonly IEnumerable<Role> _roles = Fixture.CreateMany<Role>();
        private readonly RoleRequestBundle _roleRequestBundle = Fixture.Create<RoleRequestBundle>();
        private readonly GuildRequestBundle _guildRequestBundle = Fixture.Create<GuildRequestBundle>();
        private readonly RoleGuildSettingsRequest _roleSettingsRequest = Fixture.Create<RoleGuildSettingsRequest>();
        private readonly HttpResponseMessage _successMessage = new HttpResponseMessage(HttpStatusCode.OK);
        private readonly HttpResponseMessage _notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        private readonly ExceptionWrapper _wrapper = new ExceptionWrapper(HttpStatusCode.NotFound);
        private readonly Guild _notSetupGuild;
        private readonly Guild _setupGuild;

        public InitialStartupHandlerTests()
        {
            _discordClient = Substitute.For<IInjhinuityDiscordClient>();
            _reactionRoleEmbedService = Substitute.For<IReactionRoleEmbedService>();
            _guildRequester = Substitute.For<IGuildRequester>();
            _guildBundleFactory = Substitute.For<IGuildBundleFactory>();
            _roleRequester = Substitute.For<IRoleRequester>();
            _roleBundleFactory = Substitute.For<IRoleBundleFactory>();
            _deserializer = Substitute.For<IApiReponseDeserializer>();
            _textChannel = Substitute.For<ITextChannel>();
            _userMessage = Substitute.For<IUserMessage>();
            _guild = Substitute.For<IGuild>();

            _deserializer.StrictDeserializeAsync<ExceptionWrapper>(default).ReturnsForAnyArgs(_wrapper);
            _deserializer.StrictDeserializeAndAdaptEnumerableAsync<RoleResponse, Role>(_successMessage).Returns(_roles);
            _guildBundleFactory.ReturnsForAll(_guildRequestBundle);
            _roleBundleFactory.ReturnsForAll(_roleRequestBundle);
            _textChannel.GetMessageAsync(Arg.Any<ulong>()).Returns(_userMessage);
            _guild.GetTextChannelAsync(Arg.Any<ulong>()).Returns(_textChannel);
            _guild.Id.Returns(0UL);
            _discordClient.Guilds.Returns(new[] { _guild });

            _notSetupGuild = Fixture.Create<Guild>() with { RoleSettings = new RoleGuildSettings("", "", "") };
            _setupGuild = Fixture.Create<Guild>() with { RoleSettings = new RoleGuildSettings(_channelId.ToString(), _messageId.ToString(), "") };

            _subject = new InitialStartupHandler(_discordClient, _reactionRoleEmbedService, _guildRequester,
                _guildBundleFactory, _roleRequester, _roleBundleFactory, _deserializer);
        }

        // TODO: Adjust tests once dm/admin logging is fully implemented
        [Fact]
        public async Task ExecuteColdStartupAsync_WithFirstTimeWithoutFailures_ThenExecutes()
        {
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_notFoundMessage);
            _guildRequester.ExecuteAsync(ApiAction.Post, _guildRequestBundle).Returns(_successMessage);

            await _subject.ExecuteColdStartupAsync();

            await _guildRequester.Received().ExecuteAsync(ApiAction.Post, _guildRequestBundle);
        }

        [Fact]
        public async Task ExecuteColdStartupAsync_WithSecondTime_ThenDoesntExecute()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);

            await _subject.ExecuteColdStartupAsync();
            _guildRequester.ClearReceivedCalls();

            await _subject.ExecuteColdStartupAsync();

            await _guildRequester.DidNotReceive().ExecuteAsync(ApiAction.Get, _guildRequestBundle);
            await _guildRequester.DidNotReceive().ExecuteAsync(ApiAction.Post, _guildRequestBundle);
        }

        // TODO: Adjust tests once dm/admin logging is fully implemented
        [Fact]
        public async Task ExecuteWarmStartupAsync_WithAllSuccess_ThenExecutes()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);

            await _subject.ExecuteWarmStartupAsync();

            await _reactionRoleEmbedService.Received().InitializeFromMessageAsync(_guild, _userMessage, _roles);
        }
    }
}
