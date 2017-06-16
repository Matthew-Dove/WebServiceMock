using System;
using System.Configuration;
using System.Net;

namespace WebServiceMock.Services
{
    /// <summary>
    /// Holds values for the program that change depending on the environment its running in.
    /// <para>No need to unit test this (no business logic), only integration testing.</para>
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// The base URL to listen to requests on.
        /// <para>The AppSetting key is Environment:BaseUrl, if the key isn't found, the value is set to http://localhost:8080.</para>
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// The status code to return from the mock controller when no matching rule is found.
        /// <para>For example you might define the default as 200 (OK), so you always get valid responses without having to add rules for all the mock endpoints.</para>
        /// <para>The AppSetting key is Defaults:StatusCode, if the key isn't found, the default value is set to 404.</para>
        /// </summary>
        HttpStatusCode MockStatusCode { get; }
    }

    public class ConfigService : IConfigService
    {
        public string BaseUrl { get { return GetAppSetting("Environment:BaseUrl", "http://localhost:8080"); } }

        public HttpStatusCode MockStatusCode { get { return GetAppSetting("Defaults:StatusCode", x => (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), x), HttpStatusCode.NotFound); } }

        private T GetAppSetting<T>(string key, Func<string, T> bind, T @default = default(T)) where T : struct
        {
            string value = GetAppSetting(key);
            return value == null ? @default : bind(value);
        }

        private string GetAppSetting(string key, string @default = null)
        {
            return ConfigurationManager.AppSettings[key] ?? @default;
        }
    }
}
