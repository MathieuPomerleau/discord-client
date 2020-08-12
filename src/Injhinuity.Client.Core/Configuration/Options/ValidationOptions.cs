namespace Injhinuity.Client.Core.Configuration.Options
{
    public class ValidationOptions : INullableOption
    {
        public static string OptionName => "Validation";

        public CommandValidationOptions? Command { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (Command is null)
                result.AddValueToResult(OptionName, CommandValidationOptions.OptionName);
            else
                Command.ContainsNull(result);
        }
    }
}
