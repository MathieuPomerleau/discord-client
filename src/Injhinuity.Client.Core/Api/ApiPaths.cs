namespace Injhinuity.Client.Core.Api
{
    public static class BasePath
    {
        public const string Api = "/api";
    }

    public static class CommandPath
    {
        public const string Delete = BasePath.Api + "/guild/{0}/command/{1}";
        public const string Get = BasePath.Api + "/guild/{0}/command/{1}";
        public const string GetAll = BasePath.Api + "/guild/{0}/commands";
        public const string Post = BasePath.Api + "/guild/{0}/commands";
        public const string Put = BasePath.Api + "/guild/{0}/commands";
    }

    public static class RolePath
    {
        public const string Delete = BasePath.Api + "/guild/{0}/role/{1}";
        public const string Get = BasePath.Api + "/guild/{0}/role/{1}";
        public const string GetAll = BasePath.Api + "/guild/{0}/roles";
        public const string Post = BasePath.Api + "/guild/{0}/roles";
    }

    public static class GuildPath
    {
        public const string Get = BasePath.Api + "/guild/{0}";
        public const string GetAll = BasePath.Api + "/guilds";
        public const string Post = BasePath.Api + "/guilds";
        public const string Put = BasePath.Api + "/guilds";
    }
}
