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
using Injhinuity.Client.Services.Mappers;
using Injhinuity.Client.Services.Requesters;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Modules
{
    public class CommandModuleTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly CommandModule _subject;

        private readonly string _name = Fixture.Create<string>();
        private readonly string _body = Fixture.Create<string>();
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private readonly IEnumerable<Command> _commands = Fixture.CreateMany<Command>();
        private readonly CommandRequestBundle _requestBundle = Fixture.Create<CommandRequestBundle>();
        private readonly HttpResponseMessage _successMessage = new HttpResponseMessage(HttpStatusCode.OK);
        private readonly HttpResponseMessage _notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        private readonly ExceptionWrapper _wrapper = new ExceptionWrapper(HttpStatusCode.NotFound);
        private readonly InjhinuityCommandResult _commandResult = new InjhinuityCommandResult();
        private readonly CommandResource _commandResource = Fixture.Create<CommandResource>();

        private readonly ICommandRequester _requester;
        private readonly ICommandBundleFactory _bundleFactory;
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly ICommandEmbedBuilderFactory _embedBuilderFactory;
        private readonly ICommandValidator _validator;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IValidationResourceFactory _validationResourceFactory;
        private readonly IInjhinuityMapper _mapper;
        private readonly IInjhinuityCommandContext _context;
        private readonly IPageReactionEmbed _pageReactionEmbed;
        private readonly IGuild _guild;
        private readonly IValidationResult _validationResult;

        public CommandModuleTests()
        {
            _requester = Substitute.For<ICommandRequester>();
            _bundleFactory = Substitute.For<ICommandBundleFactory>();
            _resultBuilder = Substitute.For<ICommandResultBuilder>();
            _embedBuilderFactoryProvider = Substitute.For<IEmbedBuilderFactoryProvider>();
            _embedBuilderFactory = Substitute.For<ICommandEmbedBuilderFactory>();
            _validator = Substitute.For<ICommandValidator>();
            _deserializer = Substitute.For<IApiReponseDeserializer>();
            _reactionEmbedFactory = Substitute.For<IReactionEmbedFactory>();
            _commandContextFactory = Substitute.For<IInjhinuityCommandContextFactory>();
            _validationResourceFactory = Substitute.For<IValidationResourceFactory>();
            _mapper = Substitute.For<IInjhinuityMapper>();
            _context = Substitute.For<IInjhinuityCommandContext>();
            _pageReactionEmbed = Substitute.For<IPageReactionEmbed>();
            _guild = Substitute.For<IGuild>();
            _validationResult = Substitute.For<IValidationResult>();

            _bundleFactory.ReturnsForAll(_requestBundle);
            _resultBuilder.ReturnsForAll(_resultBuilder);
            _resultBuilder.Build().Returns(_commandResult);
            _embedBuilderFactoryProvider.Command.Returns(_embedBuilderFactory);
            _embedBuilderFactory.ReturnsForAll(_embedBuilder);
            _validator.Validate(default).ReturnsForAnyArgs(_validationResult);
            _deserializer.StrictDeserializeAndAdaptEnumerableAsync<CommandResponse, Command>(default).Returns(_commands);
            _deserializer.StrictDeserializeAsync<ExceptionWrapper>(default).ReturnsForAnyArgs(_wrapper);
            _reactionEmbedFactory.CreatePageReactionEmbed(default, default).ReturnsForAnyArgs(_pageReactionEmbed);
            _commandContextFactory.Create(default).ReturnsForAnyArgs(_context);
            _validationResourceFactory.CreateCommand(default, default).Returns(_commandResource);
            _context.Guild.Returns(_guild);
            _guild.Id.Returns(0UL);

            _subject = new CommandModule(_requester, _bundleFactory, _resultBuilder, _embedBuilderFactoryProvider, _validator,
                _deserializer, _reactionEmbedFactory, _commandContextFactory, _validationResourceFactory, _mapper);
        }

        [Fact]
        public async Task CreateAsync_WithAllSuccess_ThenReturnSuccessResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.CreateAsync(_name, _body);

            _embedBuilderFactory.Received().CreateCreateSuccess(_name, _body);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.Ok);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.CreateAsync(_name, _body);

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidationResultIsFailure_ThenReturnFailureResult()
        {
            _validationResult.ValidationCode.Returns(ValidationCode.ValidationError);
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.CreateAsync(_name, _body);

            _embedBuilderFactory.Received().CreateFailure(_validationResult);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteAsync_WithApiResultIsSuccess_ThenReturnSuccessResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.DeleteAsync(_name);

            _embedBuilderFactory.Received().CreateDeleteSuccess(_name);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task DeleteAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.DeleteAsync(_name);

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task UpdateAsync_WithApiResultIsSuccess_ThenReturnSuccessResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.UpdateAsync(_name, _body);

            _embedBuilderFactory.Received().CreateUpdateSuccess(_name, _body);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task UpdateAsync_WithApiResultIsFailure_ThenReturnFailureResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.UpdateAsync(_name, _body);

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task GetAllAsync_WithApiResultIsSuccess_ThenReturnSuccessResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_successMessage);

            var result = await _subject.GetAllAsync();

            _embedBuilderFactory.Received().CreateGetAllSuccess();
            AssertResultAndReactionEmbedBuilder(result);
        }

        [Fact]
        public async Task GetAllAsync_WithResultIsFailure_ThenReturnFailureResult()
        {
            _requester.ExecuteAsync(Arg.Any<ApiAction>(), _requestBundle).Returns(_notFoundMessage);

            var result = await _subject.GetAllAsync();

            _embedBuilderFactory.Received().CreateFailure(_wrapper);
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

        private void AssertResultAndReactionEmbedBuilder(RuntimeResult result)
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
