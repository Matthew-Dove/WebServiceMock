using System;

namespace Tests.Integration.WebServiceMock.Models
{
    [Flags]
    public enum HttpVerbModel
    {
        GET = 2,
        POST = 4,
        PUT = 8,
        DELETE = 16
    }
}
