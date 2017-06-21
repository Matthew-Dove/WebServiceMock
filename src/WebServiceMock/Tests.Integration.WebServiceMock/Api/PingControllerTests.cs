using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests.Integration.WebServiceMock.Api
{
    [TestClass]
    public class PingControllerTests
    {
        [TestMethod]
        public async Task PingReturnsPong()
        {
            var get = await ApiService.GetAsync<string>("api/ping");
            Assert.AreEqual("Pong", get.Response);
        }
    }
}
