using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Results;
using Injhinuity.Client.Modules;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Modules
{
    public class InformationModuleTests
    {
        private readonly InformationModule _subject;
        private readonly InjhinuityCommandResult _result = new InjhinuityCommandResult();
        private readonly VersionConfig _version = new VersionConfig("0");
        private readonly Embed _embed = new EmbedBuilder().Build();

        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IEmbedBuilder _embedBuilder;
        private readonly IClientConfig _clientConfig;

        public InformationModuleTests()
        {
            _resultBuilder = Substitute.For<ICommandResultBuilder>();
            _embedBuilder = Substitute.For<IEmbedBuilder>();
            _clientConfig = Substitute.For<IClientConfig>();

            _clientConfig.Version.Returns(_version);
            _embedBuilder.Build().Returns(_embed);
            _embedBuilder.ReturnsForAll(_embedBuilder);
            _resultBuilder.Build().Returns(_result);
            _resultBuilder.ReturnsForAll(_resultBuilder);

            _subject = new InformationModule(_resultBuilder, _embedBuilder, _clientConfig);
        }

        [Fact]
        public async Task InfoAsync_WhenCalled_ThenReturnsTheRightResult()
        {
            var result = (InjhinuityCommandResult) await _subject.InfoAsync();

            using var scope = new AssertionScope();
            _embedBuilder.Received().WithTitle("Information");
            _embedBuilder.Received().AddField("Current Version", _version.VersionNo, true);
            _embedBuilder.Received().WithColor(Color.DarkRed);
            _embedBuilder.Received().WithTimestamp();
            _embedBuilder.Received().Build();

            _resultBuilder.Received().WithEmbed(_embed);
            _resultBuilder.Received().Build();

            result.Should().BeOfType<InjhinuityCommandResult>();
        }
    }
}
