using Microsoft.Owin.Hosting;
using System;

namespace WebServiceMock.Core
{
    public static class WebServer
    {
        private readonly static object _padLock = new object();
        private static IDisposable _handle = null;

        /// <summary>
        /// Creates a web server on the default address (http://localhost:8080).
        /// <para>It's expected only one instance is running at a time.</para>
        /// </summary>
        /// <returns>A handle for the web server.</returns>
        public static IDisposable GetHandle() => GetHandle("http://localhost:8080");

        /// <summary>
        /// Creates a web server on the specified address.
        /// <para>It's expected only one instance is running at a time.</para>
        /// </summary>
        /// <param name="baseUrl">The base URL to listen to requests on. e.g. http://localhost:8080</param>
        /// <returns>A handle for the web server.</returns>
        public static IDisposable GetHandle(string baseUrl)
        {
            if (_handle == null)
            {
                lock (_padLock)
                {
                    if (_handle == null)
                    {
                        baseUrl = string.IsNullOrEmpty(baseUrl) ? "http://localhost:8080" : baseUrl;
                        _handle = WebApp.Start<Startup>(baseUrl);
                    }
                }
            }

            return new DisposableModel(_padLock, () => _handle?.Dispose());
        }

        private struct DisposableModel : IDisposable
        {
            private readonly object _padLock;
            private readonly Action _disposeHandle;
            private bool _requiresDisposing;
            
            public DisposableModel(object padLock, Action disposeHandle)
            {
                _padLock = padLock;
                _disposeHandle = disposeHandle;
                _requiresDisposing = true;
            }

            public void Dispose()
            {
                if (_requiresDisposing)
                {
                    lock (_padLock)
                    {
                        if (_requiresDisposing)
                        {
                            _disposeHandle();
                            _requiresDisposing = false;
                        }
                    }
                }
            }
        }
    }
}
