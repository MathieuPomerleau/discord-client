using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FluentAssertions;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Modules;
using Injhinuity.Client.Services.Api;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Modules
{
    public class AdminModuleTests
    {
        private readonly AdminModule _subject;

        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private readonly InjhinuityCommandResult _commandResult = new InjhinuityCommandResult();
        private readonly string _username = "username";
        private readonly IUser _user;
        private readonly IGuild _guild;
        private readonly IGuildUser _guildUser;
        private readonly IInjhinuityCommandContext _context;

        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly IAdminEmbedBuilderFactory _embedBuilderFactory;

        public AdminModuleTests()
        {
            _user = Substitute.For<IUser>();
            _guild = Substitute.For<IGuild>();
            _guildUser = Substitute.For<IGuildUser>();
            _context = Substitute.For<IInjhinuityCommandContext>();

            _commandContextFactory = Substitute.For<IInjhinuityCommandContextFactory>();
            _deserializer = Substitute.For<IApiReponseDeserializer>();
            _resultBuilder = Substitute.For<ICommandResultBuilder>();
            _embedBuilderFactoryProvider = Substitute.For<IEmbedBuilderFactoryProvider>();
            _embedBuilderFactory = Substitute.For<IAdminEmbedBuilderFactory>();

            _resultBuilder.ReturnsForAll(_resultBuilder);
            _resultBuilder.Build().Returns(_commandResult);
            _resultBuilder.Build().Returns(_commandResult);
            _embedBuilderFactoryProvider.Admin.Returns(_embedBuilderFactory);
            _embedBuilderFactory.ReturnsForAll(_embedBuilder);
            _commandContextFactory.Create(default).ReturnsForAnyArgs(_context);
            _guild.GetUserAsync(_user.Id).Returns(_guildUser);
            _context.Guild.Returns(_guild);
            _guild.Id.Returns(0UL);
            _user.Username.Returns(_username);
            _user.Id.Returns(0UL);

            _subject = new AdminModule(_commandContextFactory, _deserializer, _resultBuilder, _embedBuilderFactoryProvider);
        }

        [Fact]
        public async Task BanAsync_ThenBansUser()
        {
            var result = await _subject.BanAsync(_user);

            await _guild.Received().AddBanAsync(_user);
            _embedBuilderFactory.Received().CreateBanSuccess(_username);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task UnbanAsync_ThenUnbansUser()
        {
            var result = await _subject.UnbanAsync(_user);

            await _guild.Received().RemoveBanAsync(_user);
            _embedBuilderFactory.Received().CreateUnbanSuccess(_username);
            AssertResultAndEmbedBuilder(result);
        }

        [Fact]
        public async Task KickAsync_ThenKicksUser()
        {
            var result = await _subject.KickAsync(_user);

            await _guildUser.Received().KickAsync();
            _embedBuilderFactory.Received().CreateKickSuccess(_username);
            AssertResultAndEmbedBuilder(result);
        }

        // TODO: Write test once muterole is implemented
        /*[Fact]
        public async Task MuteAsync_ThenMutesUser()
        {
            var result = await _subject.MuteAsync(_user);

            _embedBuilderFactory.Received().CreateMuteSuccess(_username);
            AssertResultAndEmbedBuilder(result);
        }*/

        // TODO: Write test once muterole is implemented
        /*[Fact]
        public async Task UnmuteAsync_ThenUnmutesUser()
        {
            var result = await _subject.UnmuteAsync(_user);

            _embedBuilderFactory.Received().CreateUnmuteSuccess(_username);
            AssertResultAndEmbedBuilder(result);
        }*/

        [Fact]
        public async Task RoleNotFoundAsync_ThenSendsAnErrorEmbed()
        {
            var result = await _subject.UserNotFoundAsync("some string");

            _embedBuilderFactory.Received().CreateUserNotValidFailure();
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
    }
}
