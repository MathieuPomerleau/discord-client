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
    public class RoleEmbedBuilderFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IRoleEmbedBuilderFactory _subject;

        private readonly string _name = Fixture.Create<string>();
        private readonly ExceptionWrapper _wrapper = Fixture.Create<ExceptionWrapper>();
        private readonly IValidationResult _validationResult = new ValidationResult(Core.Validation.Enums.ValidationCode.ValidationError, "message");

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public RoleEmbedBuilderFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new RoleEmbedBuilderFactory(_embedBuilder);
        }

        [Fact]
        public void CreateCreateSuccessEmbedBuilder_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateCreateSuccessEmbedBuilder(_name);

            AssertSuccessEmbedBuilder();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeCreate, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateDeleteSuccessEmbedBuilder_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateDeleteSuccessEmbedBuilder(_name);

            AssertSuccessEmbedBuilder();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeDelete, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateGetAllSuccessEmbedBuilder_WhenCalledWithRoles_ThenReturnsABuiltEmbed()
        {
            _subject.CreateGetAllSuccessEmbedBuilder();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(RoleResources.TitlePlural);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.List);
            _embedBuilder.Received().WithColor(Color.Orange);
            _embedBuilder.Received().WithTimestamp();
        }

        [Fact]
        public void CreateFailureEmbedBuilder_WhenCalledWithAnExceptionWrapper_ThenReturnsABuiltEmbed()
        {
            _subject.CreateFailureEmbedBuilder(_wrapper);

            AssertFailureEmbedBuilder();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleReason, _wrapper.Reason);
        }

        [Fact]
        public void CreateFailureEmbedBuilder_WhenCalledWithAValidationResult_ThenReturnsABuiltEmbed()
        {
            _subject.CreateFailureEmbedBuilder(_validationResult);

            AssertFailureEmbedBuilder();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleErrorCode, _validationResult.ValidationCode, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleReason, _validationResult.Message);
        }

        private void AssertSuccessEmbedBuilder()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultSuccess, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(RoleResources.Title);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().WithTimestamp();
        }

        private void AssertFailureEmbedBuilder()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultFailure, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(RoleResources.Title);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().WithTimestamp();
        }
    }
}
