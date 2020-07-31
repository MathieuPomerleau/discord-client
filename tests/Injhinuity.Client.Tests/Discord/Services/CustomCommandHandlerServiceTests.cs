using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Discord.Commands;
using FluentAssertions;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Managers;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.EmbedFactories;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Services
{
    public class CustomCommandHandlerServiceTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly ICustomCommandHandlerService _subject;

        private readonly Embed _embed = new EmbedBuilder().Build();
        private readonly Command _command = _fixture.Create<Command>();
        private readonly CommandRequestBundle _requestPackage = _fixture.Create<CommandRequestBundle>();
        private readonly HttpResponseMessage _successMessage = new HttpResponseMessage(HttpStatusCode.OK);
        private readonly HttpResponseMessage _notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        private ExceptionWrapper _wrapper;

        private readonly ICommandRequester _requester;
        private readonly ICommandPackageFactory _packageFactory;
        private readonly ICommandEmbedFactory _embedFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly IChannelManager _channelManager;
        private readonly ICommandContext _commandContext;
        private readonly ICommandExclusionService _commandExclusionService;

        public CustomCommandHandlerServiceTests()
        {
            _requester = Substitute.For<ICommandRequester>();
            _packageFactory = Substitute.For<ICommandPackageFactory>();
            _embedFactory = Substitute.For<ICommandEmbedFactory>();
            _deserializer = Substitute.For<IApiReponseDeserializer>();
            _channelManager = Substitute.For<IChannelManager>();
            _commandContext = Substitute.For<ICommandContext>();
            _commandExclusionService = Substitute.For<ICommandExclusionService>();

            _packageFactory.Create(default).ReturnsForAnyArgs(_requestPackage);
            _deserializer.DeserializeAndAdaptAsync<CommandResponse, Command>(default).ReturnsForAnyArgs(_command);
            _embedFactory.CreateCustomFailureEmbed(default).ReturnsForAnyArgs(_embed);
            _commandExclusionService.IsExcluded(default).ReturnsForAnyArgs(false);

            _subject = new CustomCommandHandlerService(_requester, _packageFactory, _embedFactory, _deserializer, _channelManager, _commandExclusionService);
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WhenCalledWithAMessageWithSpaces_ThenReturnsFalse()
        {
            var message = "aaa aaa";

            var result = await _subject.TryHandlingCustomCommand(_commandContext, message);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WhenCalledWithAnExcludedCommand_ThenReturnsFalse()
        {
            var message = "aaaa";
            _commandExclusionService.IsExcluded(message).Returns(true);

            var result = await _subject.TryHandlingCustomCommand(_commandContext, message);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WhenCalledWithProperMessageAndCommandIsFound_ThenCallsTheChannelManagerAndReturnsTrue()
        {
            _requester.ExecuteAsync(default, default).ReturnsForAnyArgs(_successMessage);
            var message = "aaa";

            var result = await _subject.TryHandlingCustomCommand(_commandContext, message);

            await _channelManager.Received().SendMessageAsync(_commandContext, _command.Body);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task TryHandlingCustomCommand_WhenCalledWithProperMessageAndHttpStatusCodeIsntSuccess_ThenCallsTheChannelManagerAndReturnsTrue()
        {
            _wrapper = new ExceptionWrapper { StatusCode = HttpStatusCode.NotFound };
            _deserializer.DeserializeAsync<ExceptionWrapper>(default).ReturnsForAnyArgs(_wrapper);
            _requester.ExecuteAsync(default, default).ReturnsForAnyArgs(_notFoundMessage);
            var message = "aaa";

            var result = await _subject.TryHandlingCustomCommand(_commandContext, message);

            await _channelManager.Received().SendEmbedMessageAsync(_commandContext, _embed);
            result.Should().BeTrue();
        }
    }
}
