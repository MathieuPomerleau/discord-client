namespace Injhinuity.Client.Core.Configuration.Options
{
    public class CommandValidationOptions : INullableOption
    {
        public static string OptionName => "CommandValidation";

        public long? CommandNameMaxLength { get; set; }
        public long? CommandBodyMaxLength { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (CommandNameMaxLength is null)
                result.AddValueToResult(OptionName, "CommandNameMaxLength");

            if (CommandBodyMaxLength is null)
                result.AddValueToResult(OptionName, "CommandBodyMaxLength");
        }
    }
}
