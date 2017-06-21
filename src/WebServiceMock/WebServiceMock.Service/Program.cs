using System;
using System.ServiceProcess;

namespace WebServiceMock.Service
{
    static class Program
    {
        static void Main()
        {
            try
            {
                ServiceBase.Run(new WebMockService());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
