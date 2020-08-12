namespace Injhinuity.Client.Core.Configuration.Options
{
    public class DiscordOptions : INullableOption
    {
        public static string OptionName => "Discord";

        public string? Token { get; set; }
        public char? Prefix { get; set; }

        public void ContainsNull(NullableOptionsResult result)
        {
            if (Token is null)
                result.AddValueToResult(OptionName, "Token");

            if (Prefix is null)
                result.AddValueToResult(OptionName, "Prefix");
        }
    }
}
