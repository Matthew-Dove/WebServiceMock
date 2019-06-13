namespace WebServiceMock.Api.Models
{
    /// <summary>The supported HTTP methods for mocking.</summary>
    public enum Verb
    {
        /// <summary>Represents the HTTP GET protocol method.</summary>
        GET = 1, // Start at 1, so the default int (0) isn't accidentally used.

        /// <summary>Represents the HTTP GET protocol method.</summary>
        POST,

        /// <summary>Represents the HTTP GET protocol method.</summary>
        PUT,

        /// <summary>Represents the HTTP GET protocol method.</summary>
        DELETE
    }
}
