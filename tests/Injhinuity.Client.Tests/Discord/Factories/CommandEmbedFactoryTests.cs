using System.Collections.Generic;
using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Model.Domain;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Factories
{
    public class CommandEmbedFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly ICommandEmbedFactory _subject;

        private readonly string _name = Fixture.Create<string>();
        private readonly string _body = Fixture.Create<string>();
        private readonly ExceptionWrapper _wrapper = Fixture.Create<ExceptionWrapper>();
        private readonly IEnumerable<Command> _commands = Fixture.CreateMany<Command>();

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public CommandEmbedFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new CommandEmbedFactory(_embedBuilder);
        }

        [Fact]
        public void CreateCreateSuccessEmbedBuilder_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCreateSuccessEmbedBuilder(_name, _body);

            AssertSuccessEmbedBuilder();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeCreate, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleContent, _body, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateDeleteSuccessEmbedBuilder_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateDeleteSuccessEmbedBuilder(_name);

            AssertSuccessEmbedBuilder();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeDelete, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateUpdateSuccessEmbedBuilder_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCreateSuccessEmbedBuilder(_name, _body);

            AssertSuccessEmbedBuilder();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeCreate, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleContent, _body, true);
            _embedBuilder.Received().AddField(CommandResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateGetAllSuccessEmbedBuilder_WhenCalledWithCommands_ThenReturnsABuiltEmbed()
        {
            _subject.CreateGetAllSuccessEmbedBuilder();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(CommandResources.TitlePlural);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.List);
            _embedBuilder.Received().WithColor(Color.Orange);
            _embedBuilder.Received().WithTimestamp();
        }

        [Fact]
        public void CreateFailureEmbedBuilder_WhenCalled_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateFailureEmbedBuilder(_wrapper);

            AssertFailureEmbedBuilder();
            _embedBuilder.AddField(CommandResources.FieldTitleApiErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.AddField(CommandResources.FieldTitleReason, _wrapper.Reason);
        }

        [Fact]
        public void CreateCustomFailureEmbedBuilder_WhenCalled_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCustomFailureEmbedBuilder(_wrapper);

            AssertFailureEmbedBuilder();
            _embedBuilder.WithTitle(CommandResources.TitleCustom);
            _embedBuilder.AddField(CommandResources.FieldTitleApiErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.AddField(CommandResources.FieldTitleReason, _wrapper.Reason ?? CommandResources.FieldValueReasonDefault);
        }

        private void AssertSuccessEmbedBuilder()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommandResources.FieldTitleResult, CommandResources.FieldValueResultSuccess, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(CommandResources.Title);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().WithTimestamp();
        }

        private void AssertFailureEmbedBuilder()
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
