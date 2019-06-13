using System;
using WebServiceMock.Api.Models;
using WebServiceMock.Core;
using WebServiceMock.Core.Models;
using WebServiceMock.Core.Services;

namespace WebServiceMock.Api.Services
{
    public sealed class WebService : IDisposable
    {
        private static bool disposedValue = false;
        private readonly static IDisposable _handle = null;
        private readonly IRuleService _ruleService = null;

        public WebService() : this(null) { }

        internal WebService(IRepositoryService repositoryService)
        {
            _ruleService = new RuleService(repositoryService ?? new RepositoryService());
        }

        static WebService()
        {
            _handle = WebServer.GetHandle("http://localhost:12345/");
        }

        public void Setup(Rule rule, Mock mock)
        {
            var ruleModel = new RuleModel
            {
                Url = rule.Path,
                Method = ConvertVerb(rule.Method),
                StatusCode = mock.StatusCode,
                ContentType = mock.ContentType,
                Body = mock.Body
            };

            _ruleService.UpsertRule(ruleModel);
        }

        private static HttpVerb ConvertVerb(Verb verb)
        {
            switch (verb)
            {
                case Verb.GET:
                    return HttpVerb.GET;
                case Verb.POST:
                    return HttpVerb.POST;
                case Verb.PUT:
                    return HttpVerb.PUT;
                case Verb.DELETE:
                    return HttpVerb.DELETE;
                default:
                    throw new ArgumentOutOfRangeException(nameof(verb), $"The HTTP verb {verb} is not an expected value.");
            }
        }

        public void Dispose() => Dispose(true);

        private static void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _handle.Dispose();
                }
                disposedValue = true;
            }
        }
    }
}
