namespace Injhinuity.Client.Core.Configuration
{
    public class ValidationConfig
    {
        public CommandValidationConfig Command { get; }

        public ValidationConfig(CommandValidationConfig commandValidationConfig)
        {
            Command = commandValidationConfig;
        }
    }
}
