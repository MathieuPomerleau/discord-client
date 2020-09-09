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
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Modules;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
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

        private readonly string _name = Fixture.Create<string>();
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private readonly IEnumerable<Role> _roles = Fixture.CreateMany<Role>();
        private readonly RoleRequestBundle _requestBundle = Fixture.Create<RoleRequestBundle>();
        private readonly HttpResponseMessage _successMessage = new HttpResponseMessage(HttpStatusCode.OK);
        private readonly HttpResponseMessage _notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        private readonly ExceptionWrapper _wrapper = new ExceptionWrapper { StatusCode = HttpStatusCode.NotFound };
        private readonly InjhinuityCommandResult _commandResult = new InjhinuityCommandResult();
        private readonly RoleResource _roleResource = Fixture.Create<RoleResource>();

        private readonly IRoleRequester _requester;
        private readonly IRoleBundleFactory _bundleFactory;
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly IRoleEmbedBuilderFactory _embedBuilderFactory;
        private readonly IRoleValidator _validator;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IValidationResourceFactory _validationResourceFactory;
        private readonly IInjhinuityCommandContext _context;
        private readonly IReactionEmbed _reactionEmbed;
        private readonly IGuild _guild;
        private readonly IGuildUser _guildUser;
        private readonly IValidationResult _validationResult;
        private readonly IRole _role;

        public RoleModuleTests()
        {
            _requester = Substitute.For<IRoleRequester>();
            _bundleFactory = Substitute.For<IRoleBundleFactory>();
            _resultBuilder = Substitute.For<ICommandResultBuilder>();
            _embedBuilderFactoryProvider = Substitute.For<IEmbedBuilderFactoryProvider>();
            _embedBuilderFactory = Substitute.For<IRoleEmbedBuilderFactory>();
            _validator = Substitute.For<IRoleValidator>();
            _deserializer = Substitute.For<IApiReponseDeserializer>();
            _reactionEmbedFactory = Substitute.For<IReactionEmbedFactory>();
            _commandContextFactory = Substitute.For<IInjhinuityCommandContextFactory>();
            _validationResourceFactory = Substitute.For<IValidationResourceFactory>();
            _context = Substitute.For<IInjhinuityCommandContext>();
            _reactionEmbed = Substitute.For<IReactionEmbed>();
            _guild = Substitute.For<IGuild>();
            _guildUser = Substitute.For<IGuildUser>();
            _validationResult = Substitute.For<IValidationResult>();
            _role = Substitute.For<IRole>();

            _bundleFactory.ReturnsForAll(_requestBundle);
            _resultBuilder.ReturnsForAll(_resultBuilder);
            _resultBuilder.Build().Returns(_commandResult);
            _embedBuilderFactoryProvider.Role.Returns(_embedBuilderFactory);
            _embedBuilderFactory.ReturnsForAll(_embedBuilder);
            _validator.Validate(default).ReturnsForAnyArgs(_validationResult);
            _deserializer.DeserializeAndAdaptEnumerableAsync<RoleResponse, Role>(default).Returns(_roles);
            _deserializer.DeserializeAsync<ExceptionWrapper>(default).ReturnsForAnyArgs(_wrapper);
            _reactionEmbedFactory.CreateListReactionEmbed(default, default).ReturnsForAnyArgs(_reactionEmbed);
            _commandContextFactory.Create(default).ReturnsForAnyArgs(_context);
            _validationResourceFactory.CreateCommand(default, default).Returns(_roleResource);
            _context.Guild.Returns(_guild);
            _context.GuildUser.Returns(_guildUser);
            _guild.Id.Returns(0UL);
            _role.Name.Returns(_name);

            _subject = new RoleModule(_requester, _bundleFactory, _embedBuilderFactoryProvider, _validator,
                _reactionEmbedFactory, _validationResourceFactory, _commandContextFactory, _deserializer, _resultBuilder);
        }

        [Fact]
        public async Task CreateAsync_WithAllSuccess_ThenReturnSuccessResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.CreateAsync(_role);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.CreateAsync(_role);

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidationResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.ValidationError);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.CreateAsync(_role);

            _embedBuilderFactory.Received().CreateFailure(_validationResult);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteAsync_WithAllSuccess_ThenReturnSuccessResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.DeleteAsync(_role);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.DeleteAsync(_role);

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteAsync_WithValidationResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.ValidationError);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.DeleteAsync(_role);

            _embedBuilderFactory.Received().CreateFailure(_validationResult);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task GetAllAsync_WithApiResultIsSuccess_ThenReturnSuccessResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.GetAllAsync();

            _embedBuilderFactory.Received().CreateGetAllSuccess();
            AssertResultAndReactionEmbed(result);
        }

        [Fact]
        public async Task GetAllAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.GetAllAsync();

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task AssignRoleAsync_WithApiResultIsSuccess_ThenReturnSuccessResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.AssignRoleAsync(_role);

            await _guildUser.Received().AddRoleAsync(_role);
            _embedBuilderFactory.Received().CreateAssignSuccess(_name);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task AssignRoleAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.AssignRoleAsync(_role);

            await _guildUser.DidNotReceive().AddRoleAsync(_role);
            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task UnassignRoleAsync_WithApiResultIsSuccess_ThenReturnSuccessResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.UnassignRoleAsync(_role);

            await _guildUser.Received().RemoveRoleAsync(_role);
            _embedBuilderFactory.Received().CreateUnassignSuccess(_name);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task UnassignRoleAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.UnassignRoleAsync(_role);

            await _guildUser.DidNotReceive().RemoveRoleAsync(_role);
            _embedBuilderFactory.Received().CreateFailure(_wrapper);
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
                _resultBuilder.WithReactionEmbed(_reactionEmbed);
                _resultBuilder.Build();
            });
        }
    }
}
