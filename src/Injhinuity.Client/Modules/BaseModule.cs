using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Services.Api;

namespace Injhinuity.Client.Modules
{
    public abstract class BaseModule : ModuleBase<SocketCommandContext>
    {
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly ICommandResultBuilder _resultBuilder;

        protected readonly IEmbedBuilderFactoryProvider EmbedBuilderFactoryProvider;

        protected BaseModule(IInjhinuityCommandContextFactory commandContextFactory, IApiReponseDeserializer deserializer, ICommandResultBuilder resultBuilder,
            IEmbedBuilderFactoryProvider embedBuilderFactoryProvider)
        {
            _commandContextFactory = commandContextFactory;
            _deserializer = deserializer;
            _resultBuilder = resultBuilder;
            EmbedBuilderFactoryProvider = embedBuilderFactoryProvider;
        }

        protected IInjhinuityCommandContext CustomContext => _commandContextFactory.Create(Context);

        protected Task<K?> DeserializeAsync<T, K>(HttpResponseMessage apiResult) where T : class where K : class =>
            _deserializer.DeserializeAndAdaptAsync<T, K>(apiResult);

        protected Task<IEnumerable<K>?> DeserializeListAsync<T, K>(HttpResponseMessage apiResult) =>
            _deserializer.DeserializeAndAdaptEnumerableAsync<T, K>(apiResult);

        protected Task<ExceptionWrapper> GetExceptionWrapperAsync(HttpResponseMessage apiResult) =>
            _deserializer.DeserializeAsync<ExceptionWrapper>(apiResult);

        protected RuntimeResult EmbedResult(EmbedBuilder embedBuilder) =>
            _resultBuilder.Create()
                .WithEmbedBuilder(embedBuilder)
                .Build();

        protected RuntimeResult ReactionEmbedResult(IReactionEmbed embed) =>
            _resultBuilder.Create()
                .WithReactionEmbed(embed)
                .Build();
    }
}
