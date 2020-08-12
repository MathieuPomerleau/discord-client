namespace Injhinuity.Client.Core.Configuration.Options
{
    public class ApiOptions : INullableOption
    {
        public static string OptionName => "Api";

        public string? BaseUrl { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (BaseUrl is null)
                result.AddValueToResult(OptionName, "BaseUrl");
        }
    }
}
