using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tests.Integration.WebServiceMock.Models;

namespace Tests.Integration.WebServiceMock
{
    static class ApiService
    {
        private readonly static HttpClient _client = null;

        static ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(Config.BaseUrl);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.FindServicePoint(new Uri(Config.BaseUrl)).ConnectionLeaseTimeout = 30000;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        public static async Task<GetModel<TResponse>> GetAsync<TResponse>(string path)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, path))
            using (var httpResponse = await _client.SendAsync(httpRequest))
            {
                var body = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TResponse>(body);
                return new GetModel<TResponse>(response, httpResponse.StatusCode);
            }
        }

        public static async Task<PostModel<TResponse>> PostAsync<TRequest, TResponse>(string path, TRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, path) { Content = content })
            using (var httpResponse = await _client.SendAsync(httpRequest))
            {
                var body = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TResponse>(body);
                return new PostModel<TResponse>(response, httpResponse.StatusCode, httpResponse.Headers.Location);
            }
        }

        public static async Task<HttpStatusCode> PutAsync<TRequest>(string path, TRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, path) { Content = content })
            using (var httpResponse = await _client.SendAsync(httpRequest))
            {
                return httpResponse.StatusCode;
            }
        }

        public static void Release() => _client.Dispose();
    }
}
