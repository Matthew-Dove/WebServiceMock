using System;
using System.Diagnostics;
using WebServiceMock.Core;

namespace WebServiceMock
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (WebServer.GetHandle(Config.BaseUrl)) // Start the web server.
                {
                    if (Environment.UserInteractive)
                    {
                        Process.Start(Config.BaseUrl);
                    }
                    Console.WriteLine(string.Concat("Listening on ", Config.BaseUrl));
                    Console.Write("Press any key to kill the server");
                    Console.ReadKey(true); // Wait for user input to kill the server.
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
