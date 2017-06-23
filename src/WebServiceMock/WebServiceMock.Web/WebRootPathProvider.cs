using Nancy;
using System;
using WebServiceMock.Core;
using WebServiceMock.Core.Models;

namespace WebServiceMock.Web
{
    public sealed class WebRootPathProvider : IRootPathProvider
    {
        private readonly static string _rootPath = null;

        static WebRootPathProvider()
        {
            _rootPath = AppDomain.CurrentDomain.BaseDirectory + "bin";
            RuntimeConfiguration.Set(HostMode.InternetInformationServices, "bin/");
        }

        public string GetRootPath() => _rootPath;
    }

    public class WebBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new WebRootPathProvider(); }
        }
    }
}