using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds.Factories;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds.Factories
{
    public class CommandEmbedBuilderFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly ICommandEmbedBuilderFactory _subject;

        private readonly string _name = Fixture.Create<string>();
        private readonly string _body = Fixture.Create<string>();
        private readonly ExceptionWrapper _wrapper = Fixture.Create<ExceptionWrapper>();
        private readonly IValidationResult _validationResult = new ValidationResult(Core.Validation.Enums.ValidationCode.ValidationError, "message");

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public CommandEmbedBuilderFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new CommandEmbedBuilderFactory(_embedBuilder);
        }

        [Fact]
        public void CreateCreateSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCreateSuccess(_name, _body);

            AssertSuccess();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeCreate, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleContent, _body, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateDeleteSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateDeleteSuccess(_name);

            AssertSuccess();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeDelete, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateUpdateSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateUpdateSuccess(_name, _body);

            AssertSuccess();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeUpdate, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleContent, _body, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateGetAllSuccess_WithCommands_ThenReturnsABuiltEmbed()
        {
            _subject.CreateGetAllSuccess();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(CommandResources.TitlePlural);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.List);
            _embedBuilder.Received().WithColor(Color.Orange);
            _embedBuilder.Received().WithTimestamp();
        }

        [Fact]
        public void CreateFailure_WithAnExceptionWrapper_ThenReturnsABuiltEmbed()
        {
            _subject.CreateFailure(_wrapper);

            AssertFailure();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleReason, _wrapper.Reason);
        }

        [Fact]
        public void CreateFailure_WithAValidationResult_ThenReturnsABuiltEmbed()
        {
            _subject.CreateFailure(_validationResult);

            AssertFailure();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleErrorCode, _validationResult.ValidationCode, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleReason, _validationResult.Message);
        }

        [Fact]
        public void CreateCustomFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCustomFailure(_wrapper);

            AssertFailure();
            _embedBuilder.Received().WithTitle(CommandResources.TitleCustom);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleReason, _wrapper.Reason);
        }

        private void AssertSuccess()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultSuccess, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(CommandResources.Title);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().WithTimestamp();
        }

        private void AssertFailure()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultFailure, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(CommandResources.Title);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().WithTimestamp();
        }
    }
}
