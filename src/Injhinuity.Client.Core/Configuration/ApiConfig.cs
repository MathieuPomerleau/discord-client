namespace Injhinuity.Client.Core.Configuration
{
    public class ApiConfig
    {
        public string BaseUrl { get; set; }

        public ApiConfig(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
    }
}
