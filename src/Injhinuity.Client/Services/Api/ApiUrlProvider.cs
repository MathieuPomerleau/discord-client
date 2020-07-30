using System;
using Injhinuity.Client.Core.Api;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain.Requests.Bundles;

namespace Injhinuity.Client.Services.Api
{
    public interface IApiUrlProvider
    {
        string GetFormattedUrl(ApiAction action, CommandRequestBundle package);
    }

    public class ApiUrlProvider : IApiUrlProvider
    {
        public string GetFormattedUrl(ApiAction action, CommandRequestBundle package) =>
            action switch
            {
                ApiAction.Delete => string.Format(CommandPath.Delete, package.GuildId, package.Request.Name),
                ApiAction.Get    => string.Format(CommandPath.Get, package.GuildId, package.Request.Name),
                ApiAction.GetAll => string.Format(CommandPath.GetAll, package.GuildId),
                ApiAction.Post   => string.Format(CommandPath.Post, package.GuildId),
                ApiAction.Put    => string.Format(CommandPath.Put, package.GuildId),
                _                => throw new ArgumentOutOfRangeException($"{action}")
            };
    }
}
