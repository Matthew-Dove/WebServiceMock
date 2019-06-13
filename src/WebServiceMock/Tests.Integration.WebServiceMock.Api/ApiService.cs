using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Tests.Integration.WebServiceMock.Api
{
    static class ApiService
    {
        private readonly static HttpClient _client = null;

        static ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:12345/");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.FindServicePoint(new Uri("http://localhost:12345/")).ConnectionLeaseTimeout = 30000;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        public static async Task<string> GetAsync(string path)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, path))
            using (var httpResponse = await _client.SendAsync(httpRequest))
            {
                return await httpResponse.Content.ReadAsStringAsync();
            }
        }

        public static void Release() => _client.Dispose();
    }
}
