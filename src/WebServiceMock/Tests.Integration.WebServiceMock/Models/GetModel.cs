using System.Net;

namespace Tests.Integration.WebServiceMock.Models
{
    class GetModel<TResponse>
    {
        public TResponse Response { get; }
        public HttpStatusCode StatusCode { get; }

        public GetModel(TResponse response, HttpStatusCode statusCode)
        {
            Response = response;
            StatusCode = statusCode;
        }
    }
}
