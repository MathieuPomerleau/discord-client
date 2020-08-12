namespace Injhinuity.Client.Core.Configuration
{
    public class CommandValidationConfig
    {
        public long CommandNameMaxLength { get; }
        public long CommandBodyMaxLength { get; }

        public CommandValidationConfig(long commandNameMaxLength, long commandBodyMaxLength)
        {
            CommandNameMaxLength = commandNameMaxLength;
            CommandBodyMaxLength = commandBodyMaxLength;
        }
    }
}
