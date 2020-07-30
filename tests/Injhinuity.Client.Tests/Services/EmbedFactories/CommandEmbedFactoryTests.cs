using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Services.EmbedFactories;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Services.EmbedFactories
{
    public class CommandEmbedFactoryTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly ICommandEmbedFactory _subject;

        private readonly string _name = _fixture.Create<string>();
        private readonly string _body = _fixture.Create<string>();
        private readonly ExceptionWrapper _wrapper = _fixture.Create<ExceptionWrapper>();
        private readonly IEnumerable<Command> _commands = _fixture.CreateMany<Command>();

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public CommandEmbedFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new CommandEmbedFactory(_embedBuilder);
        }

        [Fact]
        public void CreateCreateSuccessEmbed_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCreateSuccessEmbed(_name, _body);

            AssertSuccessEmbed();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeCreate, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleContent, _body, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateDeleteSuccessEmbed_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateDeleteSuccessEmbed(_name);

            AssertSuccessEmbed();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeDelete, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateUpdateSuccessEmbed_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCreateSuccessEmbed(_name, _body);

            AssertSuccessEmbed();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeCreate, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleContent, _body, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateGetAllSuccessEmbed_WhenCalledWithCommands_ThenReturnsABuiltEmbed()
        {
            _subject.CreateGetAllSuccessEmbed(_commands);

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(CommandResources.TitlePlural);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.List);
            _embedBuilder.Received().WithColor(Color.Orange);
            _embedBuilder.Received().WithTimestamp();
            _embedBuilder.Received(_commands.Count()).AddField(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public void CreateGetAllSuccessEmbed_WhenCalledEmptyCommandList_ThenReturnsABuiltEmbed()
        {
            _subject.CreateGetAllSuccessEmbed(null);

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(CommandResources.TitlePlural);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.List);
            _embedBuilder.Received().WithColor(Color.Orange);
            _embedBuilder.Received().WithTimestamp();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleNoCommandsFound, CommandResources.FieldValueNoCommandsFound);
        }

        [Fact]
        public void CreateFailureEmbed_WhenCalled_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateFailureEmbed(_wrapper);

            AssertFailureEmbed();
            _embedBuilder.AddField(CommandResources.FieldTitleApiErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.AddField(CommandResources.FieldTitleReason, _wrapper.Reason);
        }

        [Fact]
        public void CreateCustomFailureEmbed_WhenCalled_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCustomFailureEmbed(_wrapper);

            AssertFailureEmbed();
            _embedBuilder.WithTitle(CommandResources.TitleCustom);
            _embedBuilder.AddField(CommandResources.FieldTitleApiErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.AddField(CommandResources.FieldTitleReason, _wrapper.Reason ?? CommandResources.FieldValueReasonDefault);
        }

        private void AssertSuccessEmbed()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleResult, CommandResources.FieldValueResultSuccess, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(CommandResources.Title);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().WithTimestamp();
        }

        private void AssertFailureEmbed()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleResult, CommandResources.FieldValueResultFailure, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(CommandResources.Title);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().WithTimestamp();
        }
    }
}
