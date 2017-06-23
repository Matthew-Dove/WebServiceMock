using System;
using WebServiceMock.Core;
using WebServiceMock.Core.Models;

namespace WebServiceMock.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e) => RuntimeConfiguration.Set(HostMode.InternetInformationServices, "bin/");
    }
}