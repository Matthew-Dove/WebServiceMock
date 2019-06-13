using System.Net;

namespace WebServiceMock.Api.Models
{
    /// <summary>Describes what is returned to the external caller.</summary>
    public struct Mock
    {
        /// <summary>The status code to return when the mock URL is requested.</summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The content type to return with the response body.
        /// <para>This is optional, and can be empty.</para>
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// The body of the response to return when the mock URL is requested.
        /// <para>This is optional, and can be empty.</para>
        /// </summary>
        public string Body { get; }

        /// <summary>Describes what is returned to the external caller.</summary>
        /// <param name="statusCode">The status code to return when the mock URL is requested.</param>
        /// <param name="contentType">The content type to return with the response body (optional).</param>
        /// <param name="body">The body of the response to return when the mock URL is requested (optional).</param>
        public Mock(HttpStatusCode statusCode, string contentType = null, string body = null)
        {
            StatusCode = statusCode;
            ContentType = contentType ?? string.Empty;
            Body = body ?? string.Empty;
        }
    }
}
