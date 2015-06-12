using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Web.Http;
using WebServiceMock.Models;
using WebServiceMock.Services;

namespace WebServiceMock
{
    /// <summary>Initial configuration for self hosting.</summary>
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            FileConfiguration(app);     // Static file reads.
            app.MapSignalR();           // Real time client to server communication.
            WebApiConfiguration(app);   // WebApi to handle requests from other code (no mean't for human consumption).
            app.UseNancy();             // Nancy for serving webpages to humans, goes last as it will catch all unhandled requests (returning a 404 page).
        }

        private void FileConfiguration(IAppBuilder app)
        {
            // So we can serve JavaScript, and CSS files.
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem("Content"),
                RequestPath = new PathString("/Content")
            });
        }

        private void WebApiConfiguration(IAppBuilder app)
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
}
