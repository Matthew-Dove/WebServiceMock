using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;
using WebServiceMock.Api.Models;
using WebServiceMock.Api.Services;

namespace Tests.Integration.WebServiceMock.Api.Services
{
    [TestClass]
    public class WebServiceTests
    {
        private WebService _webService = null;

        [TestInitialize]
        public void Initialize()
        {
            _webService = new WebService();
        }

        [TestMethod]
        public async Task MyTestMethod()
        {
            _webService.Setup(new Rule(Verb.GET, new Uri("/hello", UriKind.Relative)), new Mock(HttpStatusCode.OK, "text/plain", "world"));

            var body = await ApiService.GetAsync("http://localhost:12345/api/mock/hello");

            Assert.AreEqual("world", body);
        }
    }
}
