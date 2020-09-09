using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Modules;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Modules
{
    public class InformationModuleTests
    {
        private readonly InformationModule _subject;

        private readonly string _versionNo = "version";
        private readonly InjhinuityCommandResult _commandResult = new InjhinuityCommandResult();
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();

        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly IInformationEmbedBuilderFactory _embedBuilderFactory;
        private readonly IClientConfig _clientConfig;

        public InformationModuleTests()
        {
            _resultBuilder = Substitute.For<ICommandResultBuilder>();
            _embedBuilderFactoryProvider = Substitute.For<IEmbedBuilderFactoryProvider>();
            _embedBuilderFactory = Substitute.For<IInformationEmbedBuilderFactory>();
            _clientConfig = Substitute.For<IClientConfig>();

            _resultBuilder.ReturnsForAll(_resultBuilder);
            _resultBuilder.Build().Returns(_commandResult);
            _embedBuilderFactoryProvider.Information.Returns(_embedBuilderFactory);
            _embedBuilderFactory.CreateInfo(default).ReturnsForAnyArgs(_embedBuilder);
            _clientConfig.Version.Returns(new VersionConfig(_versionNo));

            _subject = new InformationModule(_resultBuilder, _embedBuilderFactoryProvider, _clientConfig);
        }

        [Fact]
        public async Task InfoAsync_ThenReturnsCommandResultWithCorrectEmbed()
        {
            var result = await _subject.InfoAsync();

            _embedBuilderFactory.Received().CreateInfo(_versionNo);
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
