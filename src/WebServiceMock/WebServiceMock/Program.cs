using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;
using WebServiceMock.Services;

namespace WebServiceMock
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var dependencyService = new DependencyService()) // Initialize the DI container.
                {
                    var configService = dependencyService.Resolve<IConfigService>();
                    using (WebApp.Start<Startup>(configService.BaseUrl)) // Start the web server.
                    {
                        Process.Start(configService.BaseUrl);
                        Console.WriteLine(string.Concat("Listening on ", configService.BaseUrl));
                        Console.Write("Press any key to kill the server");
                        Console.ReadKey(true); // Wait for user input to kill the server.
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("Error: ", ex.Message));
                Console.WriteLine(ex);
                Console.Write("Press any key to continue");
                Console.ReadKey(true);
            }
        }
    }
}
