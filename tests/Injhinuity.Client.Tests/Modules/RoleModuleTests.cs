using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Discord.Commands;
using FluentAssertions;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Entities.Resources;
using Injhinuity.Client.Core.Validation.Enums;
using Injhinuity.Client.Core.Validation.Factories;
using Injhinuity.Client.Core.Validation.Validators;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Modules;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Mappers;
using Injhinuity.Client.Services.Requesters;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Modules
{
    public class RoleModuleTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly RoleModule _subject;

        private readonly ulong _channelId = Fixture.Create<ulong>();
        private readonly ulong _messageId = Fixture.Create<ulong>();
        private readonly string _name = Fixture.Create<string>();
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private readonly IEnumerable<Role> _roles = Fixture.CreateMany<Role>();
        private readonly RoleRequestBundle _roleRequestBundle = Fixture.Create<RoleRequestBundle>();
        private readonly GuildRequestBundle _guildRequestBundle = Fixture.Create<GuildRequestBundle>();
        private readonly RoleGuildSettingsRequest _roleSettingsRequest = Fixture.Create<RoleGuildSettingsRequest>();
        private readonly RoleResource _roleResource = Fixture.Create<RoleResource>();
        private readonly HttpResponseMessage _successMessage = new HttpResponseMessage(HttpStatusCode.OK);
        private readonly HttpResponseMessage _notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        private readonly ExceptionWrapper _wrapper = new ExceptionWrapper(HttpStatusCode.NotFound);
        private readonly InjhinuityCommandResult _commandResult = new InjhinuityCommandResult();
        private readonly Guild _notSetupGuild;
        private readonly Guild _setupGuild;

        private readonly IRoleRequester _roleRequester;
        private readonly IRoleBundleFactory _roleBundleFactory;
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IReactionRoleEmbedService _reactionRoleEmbedService;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly IRoleEmbedBuilderFactory _embedBuilderFactory;
        private readonly IRoleValidator _validator;
        private readonly IGuildRequester _guildRequester;
        private readonly IGuildBundleFactory _guildBundleFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IValidationResourceFactory _validationResourceFactory;
        private readonly IInjhinuityMapper _mapper;
        private readonly IInjhinuityCommandContext _context;
        private readonly IPageReactionEmbed _pageReactionEmbed;
        private readonly IGuild _guild;
        private readonly IGuildUser _guildUser;
        private readonly ITextChannel _textChannel;
        private readonly IUserMessage _userMessage;
        private readonly IValidationResult _validationResult;
        private readonly IRole _role;

        public RoleModuleTests()
        {
            _roleRequester = Substitute.For<IRoleRequester>();
            _roleBundleFactory = Substitute.For<IRoleBundleFactory>();
            _resultBuilder = Substitute.For<ICommandResultBuilder>();
            _reactionRoleEmbedService = Substitute.For<IReactionRoleEmbedService>();
            _embedBuilderFactoryProvider = Substitute.For<IEmbedBuilderFactoryProvider>();
            _embedBuilderFactory = Substitute.For<IRoleEmbedBuilderFactory>();
            _validator = Substitute.For<IRoleValidator>();
            _guildRequester = Substitute.For<IGuildRequester>();
            _guildBundleFactory = Substitute.For<IGuildBundleFactory>();
            _deserializer = Substitute.For<IApiReponseDeserializer>();
            _reactionEmbedFactory = Substitute.For<IReactionEmbedFactory>();
            _commandContextFactory = Substitute.For<IInjhinuityCommandContextFactory>();
            _validationResourceFactory = Substitute.For<IValidationResourceFactory>();
            _mapper = Substitute.For<IInjhinuityMapper>();
            _context = Substitute.For<IInjhinuityCommandContext>();
            _pageReactionEmbed = Substitute.For<IPageReactionEmbed>();
            _guild = Substitute.For<IGuild>();
            _guildUser = Substitute.For<IGuildUser>();
            _textChannel = Substitute.For<ITextChannel>();
            _userMessage = Substitute.For<IUserMessage>();
            _validationResult = Substitute.For<IValidationResult>();
            _role = Substitute.For<IRole>();

            _roleBundleFactory.ReturnsForAll(_roleRequestBundle);
            _guildBundleFactory.ReturnsForAll(_guildRequestBundle);
            _resultBuilder.ReturnsForAll(_resultBuilder);
            _resultBuilder.Build().Returns(_commandResult);
            _embedBuilderFactoryProvider.Role.Returns(_embedBuilderFactory);
            _embedBuilderFactory.ReturnsForAll(_embedBuilder);
            _validator.Validate(default).ReturnsForAnyArgs(_validationResult);
            _deserializer.StrictDeserializeAndAdaptEnumerableAsync<RoleResponse, Role>(default).Returns(_roles);
            _deserializer.StrictDeserializeAsync<ExceptionWrapper>(default).ReturnsForAnyArgs(_wrapper);
            _deserializer.StrictDeserializeAndAdaptEnumerableAsync<RoleResponse, Role>(_successMessage).Returns(_roles);
            _mapper.StrictMap<RoleGuildSettings, RoleGuildSettingsRequest>(Arg.Any<RoleGuildSettings>()).Returns(_roleSettingsRequest);
            _reactionEmbedFactory.CreatePageReactionEmbed(default, default).ReturnsForAnyArgs(_pageReactionEmbed);
            _commandContextFactory.Create(default).ReturnsForAnyArgs(_context);
            _validationResourceFactory.CreateCommand(default, default).Returns(_roleResource);
            _context.Guild.Returns(_guild);
            _context.GuildUser.Returns(_guildUser);
            _guild.Id.Returns(0UL);
            _guild.GetTextChannelAsync(Arg.Any<ulong>()).Returns(_textChannel);
            _textChannel.GetMessageAsync(Arg.Any<ulong>()).Returns(_userMessage);
            _role.Name.Returns(_name);

            _notSetupGuild = Fixture.Create<Guild>() with { RoleSettings = new RoleGuildSettings("", "", "") };
            _setupGuild = Fixture.Create<Guild>() with { RoleSettings = new RoleGuildSettings(_channelId.ToString(), _messageId.ToString(), "") };

            _subject = new RoleModule(_roleRequester, _roleBundleFactory, _embedBuilderFactoryProvider, _validator,
                _guildRequester, _guildBundleFactory, _reactionEmbedFactory, _validationResourceFactory,
                _commandContextFactory, _deserializer, _resultBuilder, _reactionRoleEmbedService, _mapper);
        }

        [Fact]
        public async Task CreateRoleAsync_WithAllSuccessAndReactionEmbedUpdateSuccess_ThenReturnSuccessResultAndUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name);
            await _reactionRoleEmbedService.Received().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WithAllSuccessAndReactionEmbedUpdateGuildApiFailure_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WithAllSuccessAndReactionEmbedUpdateGuildNotSetupFailure_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_notSetupGuild);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WithAllSuccessAndReactionEmbedUpdateRoleChannelIsNull_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);
            _guild.GetTextChannelAsync(Arg.Any<ulong>()).Returns((ITextChannel) null);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WithAllSuccessAndReactionEmbedUpdateRoleMessageIsNull_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(ApiAction.Post, _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);
            _textChannel.GetMessageAsync(Arg.Any<ulong>()).Returns((IUserMessage) null);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WithAllSuccessAndReactionEmbedUpdateRoleGetAllApiIsFailure_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(ApiAction.GetAll, _roleRequestBundle).Returns(_notFoundMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);
            _textChannel.GetMessageAsync(Arg.Any<ulong>()).Returns((IUserMessage)null);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WithValidationResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.ValidationError);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.CreateRoleAsync("temp", _role);

            _embedBuilderFactory.Received().CreateFailure(_validationResult);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_WithAllSuccessAndReactionEmbedUpdateSuccess_ThenReturnSuccessResultAndUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);

            var result = await _subject.DeleteRoleAsync(_role);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            await _reactionRoleEmbedService.Received().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_WithAllSuccessAndReactionEmbedUpdateGuildApiFailure_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.DeleteRoleAsync(_role);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_WithAllSuccessAndReactionEmbedUpdateGuildNotSetupFailure_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_notSetupGuild);

            var result = await _subject.DeleteRoleAsync(_role);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_WithAllSuccessAndReactionEmbedUpdateRoleChannelIsNull_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);
            _guild.GetTextChannelAsync(Arg.Any<ulong>()).Returns((ITextChannel)null);

            var result = await _subject.DeleteRoleAsync(_role);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_WithAllSuccessAndReactionEmbedUpdateRoleMessageIsNull_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(ApiAction.Delete, _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);
            _textChannel.GetMessageAsync(Arg.Any<ulong>()).Returns((IUserMessage)null);

            var result = await _subject.DeleteRoleAsync(_role);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_WithAllSuccessAndReactionEmbedUpdateRoleGetAllApiIsFailure_ThenReturnSuccessResultAndDontUpdateReactionEmbed()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(ApiAction.GetAll, _roleRequestBundle).Returns(_notFoundMessage);
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);
            _textChannel.GetMessageAsync(Arg.Any<ulong>()).Returns((IUserMessage)null);

            var result = await _subject.DeleteRoleAsync(_role);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            await _reactionRoleEmbedService.DidNotReceive().InitializeFromMessageAsync(_guild, _userMessage, _roles);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.DeleteRoleAsync(_role);

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task GetRolesAsync_WithApiResultIsSuccess_ThenReturnSuccessResult()
        {
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);

            var result = await _subject.GetRolesAsync();

            _embedBuilderFactory.Received().CreateGetAllSuccess();
            AssertResultAndReactionEmbed(result);
        }

        [Fact]
        public async Task GetRolesAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.GetRolesAsync();

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task SetupRolesAsync_WithGuildApiResultIsFailure_ThenReturnFailureResult()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.SetupRolesAsync();

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task SetupRolesAsync_WithRoleSettingsAlreadySetup_ThenReturnFailureResult()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);

            var result = await _subject.SetupRolesAsync();

            _embedBuilderFactory.Received().CreateRolesAlreadySetupFailure(_channelId.ToString());
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task SetupRolesAsync_WithRoleApiResultIsFailure_ThenReturnFailureResult()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_notFoundMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_notSetupGuild);

            var result = await _subject.SetupRolesAsync();

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task SetupRolesAsync_WithGuildUpdateApiResultIsFailure_ThenReturnFailureResult()
        {
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Put, _guildRequestBundle).Returns(_notFoundMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_notSetupGuild);

            var result = await _subject.SetupRolesAsync();

            await _reactionRoleEmbedService.Received().InitializeAsync(_context, _roles);
            await _reactionRoleEmbedService.Received().ResetAsync();
            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task SetupRolesAsync_WithAllSuccess_ThenReturnResult()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_notSetupGuild);

            var result = await _subject.SetupRolesAsync();

            await _reactionRoleEmbedService.Received().InitializeAsync(_context, _roles);
            _embedBuilderFactory.Received().CreateRolesSetupSuccess();
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task ResetRoleAsync_WithGuildApiResultIsFailure_ThenReturnFailureResult()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_notFoundMessage);

            var result = await _subject.ResetRoleAsync();

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task ResetRoleAsync_WithRoleSettingsNotSetup_ThenReturnFailureResult()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_notSetupGuild);

            var result = await _subject.ResetRoleAsync();

            _embedBuilderFactory.Received().CreateRolesNotSetupFailure();
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task ResetRoleAsync_WithGuildUpdateApiResultIsFailure_ThenReturnFailureResult()
        {
            _guildRequester.ExecuteAsync(ApiAction.Get, _guildRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _guildRequester.ExecuteAsync(ApiAction.Put, _guildRequestBundle).Returns(_notFoundMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);

            var result = await _subject.ResetRoleAsync();

            await _reactionRoleEmbedService.Received().ResetAsync();
            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task ResetRoleAsync_WithAllSuccess_ThenReturnResult()
        {
            _guildRequester.ExecuteAsync(Arg.Any<ApiAction>(), _guildRequestBundle).Returns(_successMessage);
            _roleRequester.ExecuteAsync(Arg.Any<ApiAction>(), _roleRequestBundle).Returns(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(_successMessage).Returns(_setupGuild);

            var result = await _subject.ResetRoleAsync();

            await _reactionRoleEmbedService.Received().ResetAsync();
            _embedBuilderFactory.Received().CreateRolesResetSuccess();
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task RoleNotFoundAsync_ThenReturnFailureResult()
        {
            var result = await _subject.RoleNotFoundAsync(_name);

            _embedBuilderFactory.Received().CreateRoleNotFoundFailure(_name);
            AssertResultAndEmbedBuilder(result);
        }

        private void AssertResultAndEmbedBuilder(RuntimeResult result)
        {
            result.Should().Be(_commandResult);

            Received.InOrder(() =>
            {
                _resultBuilder.Create();
                _resultBuilder.WithEmbedBuilder(_embedBuilder);
                _resultBuilder.Build();
            });
        }

        private void AssertResultAndReactionEmbed(RuntimeResult result)
        {
            result.Should().Be(_commandResult);

            Received.InOrder(() =>
            {
                _resultBuilder.Create();
                _resultBuilder.WithReactionEmbed(_pageReactionEmbed);
                _resultBuilder.Build();
            });
        }
    }
}
