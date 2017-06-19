using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Tests.Integration.WebServiceMock
{
    static class ApiService
    {
        private readonly static HttpClient _client = null;

        static ApiService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.FindServicePoint(new Uri(Config.BaseUrl)).ConnectionLeaseTimeout = 30000;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        public static async Task<TResponse> GetAsync<TResponse>(string path)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, string.Concat(Config.BaseUrl, path)))
            using (var httpResponse = await _client.SendAsync(httpRequest))
            {
                var json = await httpResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(json);
            }
        }
    }
}
