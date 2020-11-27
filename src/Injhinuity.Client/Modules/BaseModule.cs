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
using Injhinuity.Client.Services.Mappers;

namespace Injhinuity.Client.Modules
{
    public abstract class BaseModule : ModuleBase<SocketCommandContext>
    {
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IInjhinuityMapper _mapper;

        protected readonly IEmbedBuilderFactoryProvider EmbedBuilderFactoryProvider;

        protected BaseModule(IInjhinuityCommandContextFactory commandContextFactory, IApiReponseDeserializer deserializer, ICommandResultBuilder resultBuilder,
            IInjhinuityMapper mapper, IEmbedBuilderFactoryProvider embedBuilderFactoryProvider)
        {
            _commandContextFactory = commandContextFactory;
            _deserializer = deserializer;
            _resultBuilder = resultBuilder;
            _mapper = mapper;
            EmbedBuilderFactoryProvider = embedBuilderFactoryProvider;
        }

        protected IInjhinuityCommandContext CustomContext => _commandContextFactory.Create(Context);

        protected Task<K> StrictDeserializeAsync<T, K>(HttpResponseMessage apiResult) where T : class where K : class =>
            _deserializer.StrictDeserializeAndAdaptAsync<T, K>(apiResult);

        protected Task<IEnumerable<K>> StrictDeserializeListAsync<T, K>(HttpResponseMessage apiResult) where T : class where K : class =>
            _deserializer.StrictDeserializeAndAdaptEnumerableAsync<T, K>(apiResult);

        protected K StrictMap<T, K>(T source) where T : class where K : class =>
            _mapper.StrictMap<T, K>(source);

        protected Task<ExceptionWrapper> GetExceptionWrapperAsync(HttpResponseMessage apiResult) =>
            _deserializer.StrictDeserializeAsync<ExceptionWrapper>(apiResult);

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
