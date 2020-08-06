using System;
using Injhinuity.Client.Core.Api;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain.Requests.Bundles;

namespace Injhinuity.Client.Services.Api
{
    public interface IApiUrlProvider
    {
        string GetFormattedUrl(ApiAction action, CommandRequestBundle bundle);
    }

    public class ApiUrlProvider : IApiUrlProvider
    {
        public string GetFormattedUrl(ApiAction action, CommandRequestBundle bundle) =>
            action switch
            {
                ApiAction.Delete => string.Format(CommandPath.Delete, bundle.GuildId, bundle.Request.Name),
                ApiAction.Get    => string.Format(CommandPath.Get, bundle.GuildId, bundle.Request.Name),
                ApiAction.GetAll => string.Format(CommandPath.GetAll, bundle.GuildId),
                ApiAction.Post   => string.Format(CommandPath.Post, bundle.GuildId),
                ApiAction.Put    => string.Format(CommandPath.Put, bundle.GuildId),
                _                => throw new ArgumentOutOfRangeException($"{action}")
            };
    }
}
