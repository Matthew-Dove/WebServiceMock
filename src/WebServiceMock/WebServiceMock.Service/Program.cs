using System.ServiceProcess;

namespace WebServiceMock.Service
{
    static class Program
    {
        static void Main() => ServiceBase.Run(new WebMockService());
    }
}
