using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using WebServiceMock.Api.Services;

namespace Tests.Integration.WebServiceMock.Api
{
    [TestClass]
    public class TestStartup
    {
        private static IDisposable _handle = null;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            StartWebServer();
        }

        #region Initialize

        static void StartWebServer() => _handle = new WebService();

        #endregion

        [AssemblyCleanup]
        public static void Cleanup()
        {
            TearDownWebServer();
            ReleaseTcpSocket();
            RemoveRulesRepository();
        }

        #region Cleanup

        static void TearDownWebServer() => _handle.Dispose();

        static void ReleaseTcpSocket() => ApiService.Release();

        static void RemoveRulesRepository()
        {
            var path = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\rules.txt");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #endregion
    }
}
