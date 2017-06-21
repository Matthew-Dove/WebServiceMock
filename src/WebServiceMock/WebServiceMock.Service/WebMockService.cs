using System;
using System.Diagnostics;
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

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("OnStart - Begin", EventLogEntryType.Information);

            try
            {
                _handle = WebServer.GetHandle(Config.BaseUrl); // Start the web server.
            }
            catch (Exception ex)
            {
                Log.Error(ex, EventLog);
                throw;
            }

            EventLog.WriteEntry("OnStart - End", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("OnStop - Begin", EventLogEntryType.Information);

            try
            {
                _handle?.Dispose(); // Stop the web server.
            }
            catch (Exception ex)
            {
                Log.Error(ex, EventLog);
                throw;
            }

            EventLog.WriteEntry("OnStop - End", EventLogEntryType.Information);
        }
    }
}
