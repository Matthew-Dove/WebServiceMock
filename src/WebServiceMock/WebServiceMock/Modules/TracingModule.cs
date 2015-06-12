using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceMock.Modules
{
    /// <summary>A simple page that displays trace message after trace message, as there broadcast from the Trace Hub.</summary>
    public class TracingModule : NancyModule
    {
        public TracingModule()
        {
            Get["/Tracing"] = _ => {
                return View["Tracing"];
            };
        }
    }
}
