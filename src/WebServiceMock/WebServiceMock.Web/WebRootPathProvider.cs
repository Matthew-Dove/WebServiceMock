using Nancy;
using System;

namespace WebServiceMock.Web
{
    public sealed class WebRootPathProvider : IRootPathProvider
    {
        private readonly static string _rootPath = AppDomain.CurrentDomain.BaseDirectory + "bin";
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