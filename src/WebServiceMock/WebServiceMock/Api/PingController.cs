using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebServiceMock.Hubs;
using WebServiceMock.Services;

namespace WebServiceMock.Api
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
