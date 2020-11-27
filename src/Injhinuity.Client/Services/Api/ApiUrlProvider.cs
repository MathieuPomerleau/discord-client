using System;
using Injhinuity.Client.Core.Api;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain.Requests.Bundles;

namespace Injhinuity.Client.Services.Api
{
    public interface IApiUrlProvider
    {
        string GetFormattedUrl(ApiAction action, GuildRequestBundle bundle);
        string GetFormattedUrl(ApiAction action, CommandRequestBundle bundle);
        string GetFormattedUrl(ApiAction action, RoleRequestBundle bundle);
    }

    public class ApiUrlProvider : IApiUrlProvider
    {
        public string GetFormattedUrl(ApiAction action, CommandRequestBundle bundle) =>
            action switch
            {
                ApiAction.Delete => string.Format(CommandPath.Delete, bundle.GuildId, bundle.Request!.Name),
                ApiAction.Get    => string.Format(CommandPath.Get, bundle.GuildId, bundle.Request!.Name),
                ApiAction.GetAll => string.Format(CommandPath.GetAll, bundle.GuildId),
                ApiAction.Post   => string.Format(CommandPath.Post, bundle.GuildId),
                ApiAction.Put    => string.Format(CommandPath.Put, bundle.GuildId),
                _                => throw new ArgumentOutOfRangeException($"{action}")
            };

        public string GetFormattedUrl(ApiAction action, RoleRequestBundle bundle) =>
            action switch
            {
                ApiAction.Delete => string.Format(RolePath.Delete, bundle.GuildId, bundle.Request!.Name),
                ApiAction.Get    => string.Format(RolePath.Get, bundle.GuildId, bundle.Request!.Name),
                ApiAction.GetAll => string.Format(RolePath.GetAll, bundle.GuildId),
                ApiAction.Post   => string.Format(RolePath.Post, bundle.GuildId),
                ApiAction.Put    => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException($"{action}")
            };

        public string GetFormattedUrl(ApiAction action, GuildRequestBundle bundle) =>
            action switch
            {
                ApiAction.Delete => throw new NotImplementedException(),
                ApiAction.Get    => string.Format(GuildPath.Get, bundle.Id),
                ApiAction.GetAll => string.Format(GuildPath.GetAll),
                ApiAction.Post   => string.Format(GuildPath.Post),
                ApiAction.Put    => string.Format(GuildPath.Put),
                _                => throw new ArgumentOutOfRangeException($"{action}")
            };
    }
}
