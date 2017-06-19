using System.Configuration;

namespace WebServiceMock
{
    /// <summary>Holds values for the program that change depending on the environment its running in.</summary>
    static class Config
    {
        /// <summary>
        /// The base URL to listen to requests on.
        /// <para>The AppSetting key is Environment:BaseUrl, if the key isn't found, the value is set to http://localhost:8080.</para>
        /// </summary>
        public static string BaseUrl { get { return GetAppSetting("Environment:BaseUrl"); } }

        private static string GetAppSetting(string key) => ConfigurationManager.AppSettings[key];
    }
}
