using System;
using System.Net;

namespace Tests.Integration.WebServiceMock.Models
{
    struct PostModel<TResponse>
    {
        public TResponse Response { get; }
        public HttpStatusCode StatusCode { get; }
        public Uri Location { get; }

        public PostModel(TResponse response, HttpStatusCode statusCode, Uri location)
        {
            Response = response;
            StatusCode = statusCode;
            Location = location;
        }
    }
}
