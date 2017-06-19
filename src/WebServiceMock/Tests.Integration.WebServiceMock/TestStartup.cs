using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WebServiceMock.Core;

namespace Tests.Integration.WebServiceMock
{
    [TestClass]
    public class TestStartup
    {
        private static IDisposable _handle = null;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            _handle = WebServer.GetHandle(Config.BaseUrl);
        }

        [AssemblyCleanup()]
        public static void Cleanup()
        {
            _handle.Dispose();
        }
    }
}
