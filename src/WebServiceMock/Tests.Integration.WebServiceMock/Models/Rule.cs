using System;
using System.Net;

namespace Tests.Integration.WebServiceMock.Models
{
    class Rule
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public HttpVerbModel Method { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; }
        public string Body { get; set; }
    }
}
