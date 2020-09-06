namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IEmbedBuilderFactoryProvider
    {
        IInformationEmbedBuilderFactory Information { get; }
        ICommandEmbedBuilderFactory Command { get; }
        IRoleEmbedBuilderFactory Role { get; }
    }

    public class EmbedBuilderFactoryProvider : IEmbedBuilderFactoryProvider
    {
        public EmbedBuilderFactoryProvider(IInformationEmbedBuilderFactory information, ICommandEmbedBuilderFactory command, IRoleEmbedBuilderFactory role)
        {
            Information = information;
            Command = command;
            Role = role;
        }

        public IInformationEmbedBuilderFactory Information { get; }
        public ICommandEmbedBuilderFactory Command { get; }
        public IRoleEmbedBuilderFactory Role { get; }
    }
}
