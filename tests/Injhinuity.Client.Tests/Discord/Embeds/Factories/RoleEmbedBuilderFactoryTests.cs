﻿using AutoFixture;
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
        private readonly string _rolename = Fixture.Create<string>();
        private readonly string _channelId = Fixture.Create<string>();
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
        public void CreateCreateSuccess_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateCreateSuccess(_name);

            AssertSuccess();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateDeleteSuccess_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateDeleteSuccess(_name);

            AssertSuccess();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
        }

        [Fact]
        public void CreateGetAllSuccess_WithRoles_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateGetAllSuccess();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(RoleResources.TitlePlural);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.List);
            _embedBuilder.Received().WithColor(Color.Orange);
        }

        [Fact]
        public void CreateAssignRoleSuccess_WithUsernameAndRolename_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateAssignRoleSuccess(_name, _rolename);

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(RoleResources.TitleAssign);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleUser, _name, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleRole, _rolename, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithColor(Color.Green);
        }

        [Fact]
        public void CreateUnassignRoleSuccess_WithRoles_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateUnassignRoleSuccess(_name, _rolename);

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(RoleResources.TitleUnassign);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleUser, _name, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleRole, _rolename, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithColor(Color.Green);
        }

        [Fact]
        public void CreateRolesSetupSuccess_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateRolesSetupSuccess();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(RoleResources.TitleRolesSetup);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().Build();
        }

        [Fact]
        public void CreateRolesResetSuccess_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateRolesResetSuccess();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(RoleResources.TitleRolesReset);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().Build();
        }

        [Fact]
        public void CreateReactionRole_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateReactionRole();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithTitle(RoleResources.TitlePlural);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.List);
            _embedBuilder.Received().WithColor(Color.Purple);
            _embedBuilder.Received().Build();
        }

        [Fact]
        public void CreateRoleNotFoundFailure_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateRoleNotFoundFailure(_name);

            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleName, _name);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(RoleResources.TitleRoleNotFound);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().Build();
        }

        [Fact]
        public void CreateRolesAlreadySetupFailure_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateRolesAlreadySetupFailure(_channelId);

            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleChannelId, _channelId);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(RoleResources.TitleRolesAlreadySetup);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().Build();
        }

        [Fact]
        public void CreateRolesNotSetupFailure_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateRolesNotSetupFailure();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(RoleResources.TitleRolesNotSetup);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().Build();
        }

        [Fact]
        public void CreateFailure_WithAnExceptionWrapper_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateFailure(_wrapper);

            AssertFailure();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleErrorCode, _wrapper.StatusCode, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleReason, _wrapper.Reason);
        }

        [Fact]
        public void CreateFailure_WithAValidationResult_ThenReturnsAnEmbedBuilder()
        {
            _subject.CreateFailure(_validationResult);

            AssertFailure();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleErrorCode, _validationResult.ValidationCode, true);
            _embedBuilder.Received().AddField(CommonResources.FieldTitleReason, _validationResult.Message);
        }

        private void AssertSuccess()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(RoleResources.Title);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().Build();
        }

        private void AssertFailure()
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(RoleResources.Title);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().Build();
        }
    }
}
