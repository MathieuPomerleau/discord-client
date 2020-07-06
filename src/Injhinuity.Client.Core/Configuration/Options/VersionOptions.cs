namespace Injhinuity.Client.Core.Configuration.Options
{
    public class VersionOptions : INullableOption
    {
        private const string OptionName = "Version";

        public string? VersionNo { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (VersionNo is null)
                result.AddValueToResult(OptionName, "VersionNo");
        }
    }
}
