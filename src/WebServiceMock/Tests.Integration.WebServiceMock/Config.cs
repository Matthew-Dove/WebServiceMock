using System.Configuration;

namespace Tests.Integration.WebServiceMock
{
    static class Config
    {
        /// <summary>The url to run the web server on while running integration tests.</summary>
        public static string BaseUrl { get { return GetAppSetting("WebServer:BaseUrl"); } }

        private static string GetAppSetting(string key) => ConfigurationManager.AppSettings[key];
    }
}
