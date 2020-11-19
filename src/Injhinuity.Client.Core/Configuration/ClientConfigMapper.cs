#nullable disable
using Injhinuity.Client.Core.Configuration.Options;
using Injhinuity.Client.Core.Exceptions;

namespace Injhinuity.Client.Core.Configuration
{
    public interface IClientConfigMapper
    {
        IClientConfig MapFromNullableOptions(IClientOptions options);
    }

    public class ClientConfigMapper : IClientConfigMapper
    {
        public IClientConfig MapFromNullableOptions(IClientOptions clientOptions)
        {
            if (clientOptions is not ClientOptions options)
                throw new InjhinuityException("Configuration couldn't be built, options are null");

            var result = new NullableOptionsResult();
            options.ContainsNull(result);

            if (!result.IsValid)
                throw new InjhinuityException($"The following values are missing from the configuration:\n{result}");

            var version = new VersionConfig(options.Version.VersionNo);
            var logging = new LoggingConfig(options.Logging.AppLogLevel.Value, options.Logging.DiscordLogLevel.Value);
            var discord = new DiscordConfig(options.Discord.Token, options.Discord.Prefix.Value);
            var api = new ApiConfig(options.Api.BaseUrl);
            var validation = MapValidationConfig(options.Validation);

            return new ClientConfig(version, logging, discord, api, validation);
        }

        private static ValidationConfig MapValidationConfig(ValidationOptions options)
        {
            var commandValidation = new CommandValidationConfig(options.Command.CommandNameMaxLength.Value, options.Command.CommandBodyMaxLength.Value);
            return new ValidationConfig(commandValidation);
        }
    }
}
