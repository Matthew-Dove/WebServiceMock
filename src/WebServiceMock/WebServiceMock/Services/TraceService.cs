using Microsoft.AspNet.SignalR;
using System;
using WebServiceMock.Hubs;

namespace WebServiceMock.Services
{
    /// <summary>Methods for talking to clients through the TraceHub.</summary>
    public interface ITraceService
    {
        /// <summary>Sends a log message to all connected clients.</summary>
        /// <param name="text">The message to send the clients.</param>
        void Log(string text);

        /// <summary>Sends a error message to all connected clients.</summary>
        /// <param name="ex">The error to send the clients.</param>
        void Error(Exception ex);

        /// <summary>Sends a error message to all connected clients.</summary>
        /// <param name="text">The error to send the clients.</param>
        void Error(string text);
    }

    /// <summary>No need to unit test this (no business logic), only integration testing.</summary>
    public class TraceService : ITraceService
    {
        private readonly IHubContext _traceHub = null;

        public TraceService()
        {
            _traceHub = GlobalHost.ConnectionManager.GetHubContext<TraceHub>();
        }

        public void Log(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            _traceHub.Clients.All.receiveLog(text);
        }

        public void Error(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            Error(ex.ToString());
        }

        public void Error(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            _traceHub.Clients.All.receiveError(text);
        }
    }
}
