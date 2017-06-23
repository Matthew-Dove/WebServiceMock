using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Web.Http;
using WebServiceMock.Core.Models;
using WebServiceMock.Core.Services;

[assembly: OwinStartup(typeof(WebServiceMock.Core.Startup))]
namespace WebServiceMock.Core
{
    /// <summary>Initial configuration for self hosting.</summary>
    class Startup
    {
        // By default assume the application is self-hosted.
        private static HostMode _hostMode = HostMode.Self;
        private static string _relativeRootPath = string.Empty; // The relative path to the Content folder.

        public static void SetConfiguration(HostMode hostMode, string relativeRootPath)
        {
            _hostMode = hostMode;
            _relativeRootPath = relativeRootPath;
        }

        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();           // Real time client to server communication.
            WebApiConfiguration(app);   // WebApi to handle requests from other code (no mean't for human consumption).
            app.UseNancy();             // Nancy for serving webpages to humans, goes after WEB API config as it will catch all unhandled requests (returning a 404 page).
            FileConfiguration(app);     // Static file reads, goes after Nancy so the root path is set first.
            if (_hostMode == HostMode.InternetInformationServices)
            {
                app.UseStageMarker(PipelineStage.MapHandler); // Plug into IIS so it can host the process.
            }
        }

        private static void FileConfiguration(IAppBuilder app)
        {
            // So we can serve JavaScript, and CSS files.
            app.UseStaticFiles(new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(_relativeRootPath + "Content"),
                RequestPath = new PathString("/Content"),
                OnPrepareResponse = StaticFileResponse
            });
        }

        private static void StaticFileResponse(StaticFileResponseContext fileResponse)
        {
            var cacheSeconds = 60 * 60 * 10;
            fileResponse.OwinContext.Response.Headers["Cache-Control"] = "public,max-age=" + cacheSeconds;
        }

        private static void WebApiConfiguration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            var dependencyService = new DependencyService();

            // Allow actions in api controllers to define their own routes.
            configuration.MapHttpAttributeRoutes();

            // Hook up Dependency injection for WebApi Controllers.
            configuration.DependencyResolver = dependencyService.DependencyResolver;

            // Will match any path that starts with ~/Api/Mock/*, the action is selected by the HTTP verb (used to map paths that are configurable at runtime).
            configuration.Routes.MapHttpRoute(
                "WebApiMock",
                string.Concat(RuleModel.START_URL.Substring(1, RuleModel.START_URL.Length - 1), "/{*path}"),
                defaults: new { Controller = "Mock" }
            );

            // General routing for controllers with actions (RCP style).
            configuration.Routes.MapHttpRoute(
                "WebApiAction",
                "Api/{controller}/{action}/"
            );

            // General routing for controllers with the actions selected by the HTTP verb (REST style).
            configuration.Routes.MapHttpRoute(
                "WebApi",
                "Api/{controller}/"
            );

            app.UseWebApi(configuration);
        }
    }

    public static class RuntimeConfiguration
    {
        /// <summary>Set runtime environment values specific to how the solution is hosted.</summary>
        /// <param name="hostMode">Whether the application is self-hosted or not.</param>
        /// <param name="relativeRootPath">If the root path for the content folder, and it's files.</param>
        public static void Set(HostMode hostMode, string relativeRootPath) => Startup.SetConfiguration(hostMode, relativeRootPath);
    }
}
