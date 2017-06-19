using System.Web.Http;

namespace WebServiceMock.Core.Api
{
    /// <summary>A simple Controller so you can ensure the server is up and running correctly.</summary>
    public class PingController : ApiController
    {
        public string Get()
        {
            return "Pong";
        }
    }
}
