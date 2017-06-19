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
            var result = await ApiService.GetAsync<string>("/Api/Ping");
            Assert.AreEqual("Pong", result);
        }
    }
}
