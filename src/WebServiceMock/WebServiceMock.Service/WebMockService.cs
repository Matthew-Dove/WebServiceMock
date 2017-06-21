using System;
using System.ServiceProcess;
using WebServiceMock.Core;

namespace WebServiceMock.Service
{
    public partial class WebMockService : ServiceBase
    {
        private IDisposable _handle = null;

        public WebMockService()
        {
            InitializeComponent();
            CanPauseAndContinue = false; // It doesn't make sense to pause the web server, as it can sit idle indefinitely; startup / teardown procedures are sub-second, and state is persisted across restarts.
        }

        protected override void OnStart(string[] args) => _handle = WebServer.GetHandle(Config.BaseUrl); // Start the web server.

        protected override void OnStop() => _handle?.Dispose(); // Stop the web server.
    }
}
