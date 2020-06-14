namespace Injhinuity.Client.Core.Configuration.Options
{
    public class DiscordOptions : INullableOption
    {
        public string? Token { get; set; }
        public char? Prefix { get; set; }

        public bool ContainsNull() =>
            Token is null ||
            Prefix is null;
    }
}
