using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Services
{
    public class CustomCommandHandlerServiceTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly ICustomCommandHandlerService _subject;

        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private readonly Command _command = Fixture.Create<Command>();
        private readonly Command _noBodyCommand = new Command("aaaa", null);
        private readonly CommandRequestBundle _requestBundle = Fixture.Create<CommandRequestBundle>();
        private readonly HttpResponseMessage _successMessage = new HttpResponseMessage(HttpStatusCode.OK);
        private readonly HttpResponseMessage _notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        private readonly ExceptionWrapper _wrapper = new ExceptionWrapper(HttpStatusCode.NotFound);

        private readonly ICommandRequester _requester;
        private readonly ICommandBundleFactory _bundleFactory;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly ICommandEmbedBuilderFactory _embedBuilderFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly ICommandExclusionService _commandExclusionService;
        private readonly IInjhinuityCommandContext _injhinuityContext;
        private readonly IMessageChannel _channel;

        public CustomCommandHandlerServiceTests()
        {
            _requester = Substitute.For<ICommandRequester>();
            _bundleFactory = Substitute.For<ICommandBundleFactory>();
            _embedBuilderFactoryProvider = Substitute.For<IEmbedBuilderFactoryProvider>();
            _embedBuilderFactory = Substitute.For<ICommandEmbedBuilderFactory>();
            _deserializer = Substitute.For<IApiReponseDeserializer>();
            _commandExclusionService = Substitute.For<ICommandExclusionService>();
            _injhinuityContext = Substitute.For<IInjhinuityCommandContext>();
            _channel = Substitute.For<IMessageChannel>();

            _bundleFactory.Create(default).ReturnsForAnyArgs(_requestBundle);
            _deserializer.StrictDeserializeAndAdaptAsync<CommandResponse, Command>(default).ReturnsForAnyArgs(_command);
            _embedBuilderFactoryProvider.Command.Returns(_embedBuilderFactory);
            _embedBuilderFactory.CreateCustomFailure(default).ReturnsForAnyArgs(_embedBuilder);
            _injhinuityContext.Channel.Returns(_channel);
            _commandExclusionService.IsExcluded(default).ReturnsForAnyArgs(false);

            _subject = new CustomCommandHandlerService(_requester, _bundleFactory, _embedBuilderFactoryProvider, _deserializer, _commandExclusionService);
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WithAMessageWithSpaces_ThenReturnsFalse()
        {
            var message = "aaa aaa";

            var result = await _subject.TryHandlingCustomCommand(_injhinuityContext, message);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WithAnExcludedCommand_ThenReturnsFalse()
        {
            var message = "aaaa";
            _commandExclusionService.IsExcluded(message).Returns(true);

            var result = await _subject.TryHandlingCustomCommand(_injhinuityContext, message);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WithProperMessageAndCommandIsFound_ThenSendAMessageToChannelAndReturnTrue()
        {
            _requester.ExecuteAsync(default, default).ReturnsForAnyArgs(_successMessage);
            var message = "aaa";

            var result = await _subject.TryHandlingCustomCommand(_injhinuityContext, message);

            await _channel.Received().SendMessageAsync(_command.Body);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WithProperMessageAndCommandIsFoundButWithoutABody_ThenSendDefaultMessageToChannelAndReturnTrue()
        {
            _requester.ExecuteAsync(default, default).ReturnsForAnyArgs(_successMessage);
            _deserializer.StrictDeserializeAndAdaptAsync<CommandResponse, Command>(default).ReturnsForAnyArgs(_noBodyCommand);
            var message = "aaa";

            var result = await _subject.TryHandlingCustomCommand(_injhinuityContext, message);

            await _channel.Received().SendMessageAsync(CommonResources.CommandNoBody);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WithProperMessageAndHttpStatusCodeIsntSuccess_ThenCallsTheChannelManagerAndReturnsTrue()
        {
            _deserializer.StrictDeserializeAsync<ExceptionWrapper>(default).ReturnsForAnyArgs(_wrapper);
            _requester.ExecuteAsync(default, default).ReturnsForAnyArgs(_notFoundMessage);
            var message = "aaa";

            var result = await _subject.TryHandlingCustomCommand(_injhinuityContext, message);

            _embedBuilderFactory.Received().CreateCustomFailure(_wrapper);
            await _channel.Received().SendMessageAsync(string.Empty, false, Arg.Any<Embed>());
            result.Should().BeTrue();
        }
    }
}
