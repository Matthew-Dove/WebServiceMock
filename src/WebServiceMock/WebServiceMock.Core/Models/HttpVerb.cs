using System;

namespace WebServiceMock.Core.Models
{
    /// <summary>The supported HTTP methods for mocking.</summary>
    /// <remarks>I don't use System.Net.Http.HttpMethod because Head, Options, and Trace aren't supported by the rules.</remarks>
    [Flags]
    public enum HttpVerb
    {
        /// <summary>Represents the HTTP GET protocol method.</summary>
        GET = 2,

        /// <summary>Represents the HTTP GET protocol method.</summary>
        POST = 4,

        /// <summary>Represents the HTTP GET protocol method.</summary>
        PUT = 8,

        /// <summary>Represents the HTTP GET protocol method.</summary>
        DELETE = 16
    }
}
