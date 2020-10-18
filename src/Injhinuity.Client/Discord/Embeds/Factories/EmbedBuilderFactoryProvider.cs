namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IEmbedBuilderFactoryProvider
    {
        IInformationEmbedBuilderFactory Information { get; }
        ICommandEmbedBuilderFactory Command { get; }
        IRoleEmbedBuilderFactory Role { get; }
        IAdminEmbedBuilderFactory Admin { get; }
        IPermissionEmbedBuilderFactory Permission { get; }
    }

    public class EmbedBuilderFactoryProvider : IEmbedBuilderFactoryProvider
    {
        public EmbedBuilderFactoryProvider(IInformationEmbedBuilderFactory information, ICommandEmbedBuilderFactory command, IRoleEmbedBuilderFactory role,
            IAdminEmbedBuilderFactory admin, IPermissionEmbedBuilderFactory permission)
        {
            Information = information;
            Command = command;
            Role = role;
            Admin = admin;
            Permission = permission;
        }

        public IInformationEmbedBuilderFactory Information { get; }
        public ICommandEmbedBuilderFactory Command { get; }
        public IRoleEmbedBuilderFactory Role { get; }
        public IAdminEmbedBuilderFactory Admin { get; }
        public IPermissionEmbedBuilderFactory Permission { get; }
    }
}
