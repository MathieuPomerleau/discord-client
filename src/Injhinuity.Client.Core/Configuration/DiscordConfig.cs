namespace Injhinuity.Client.Core.Configuration
{
    public class DiscordConfig
    {
        public string Token { get; set; }
        public char Prefix { get; set; }

        public DiscordConfig(string token, char prefix)
        {
            Token = token;
            Prefix = prefix;
        }
    }
}
