using System;
using System.Diagnostics;
using System.IO;

namespace WebServiceMock.Service
{
    static class Log
    {
        public static void Error(Exception ex)
        {
            LogToFile(ex);
        }

        public static void Error(Exception ex, EventLog e)
        {
            LogToFile(ex);
            LogToEventViewer(ex, e);
        }

        private static void LogToFile(Exception ex)
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
                var message = $"{DateTime.Now} - Error starting the service: {ex.Message}\r\n{ex}\r\n\r\n";
                File.AppendAllText(path, message);
            }
            catch (Exception)
            {

            }
        }

        private static void LogToEventViewer(Exception ex, EventLog e)
        {
            var message = $"{DateTime.Now} - Error starting the service: {ex.Message}\r\n{ex}\r\n\r\n";
            e.WriteEntry(message, EventLogEntryType.Error);
        }
    }
}
