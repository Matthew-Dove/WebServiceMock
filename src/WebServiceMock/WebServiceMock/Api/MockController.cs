﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebServiceMock.Models;
using WebServiceMock.Services;

namespace WebServiceMock.Api
{
    /// <summary>All requests starting with ~/Api/Mock/* are routed to this controller.</summary>
    public class MockController : ApiController
    {
        private readonly ITraceService _traceService = null;
        private readonly IRuleService _ruleService = null;
        private readonly IConfigService _configService = null;

        public MockController(ITraceService traceService, IRuleService ruleService, IConfigService configService)
        {
            _traceService = traceService;
            _ruleService = ruleService;
            _configService = configService;
        }

        public IHttpActionResult Get(string path)
        {
            return ProcessRequest(path, HttpVerb.GET);
        }

        public IHttpActionResult Post(string path)
        {
            return ProcessRequest(path, HttpVerb.POST);
        }

        public IHttpActionResult Put(string path)
        {
            return ProcessRequest(path, HttpVerb.PUT);
        }

        public IHttpActionResult Delete(string path)
        {
            return ProcessRequest(path, HttpVerb.DELETE);
        }

        private IHttpActionResult ProcessRequest(string path, HttpVerb method)
        {
            IHttpActionResult response = StatusCode(_configService.MockStatusCode); // This status code will be used as the response, when the relative url doesn't match an existing rule.
            var url = new Uri(string.Concat('/', path), UriKind.Relative);
            RuleModel rule;

            try
            {
                if (_ruleService.TryGetRule(url, method, out rule))
                {
                    response = rule.HasResponseBody ? GetResponseBody(rule) : GetStatusCode(rule);
                }
                else
                {
                    var statuscode = _configService.MockStatusCode;
                    _traceService.Log(string.Format("No rule found for {0} {1}, returning the status code {2} ({3}).", method, url.OriginalString, (int)statuscode, statuscode));
                }
            }
            catch (Exception ex)
            {
                response = InternalServerError();
                _traceService.Error(string.Format("Error getting a rule for {0} {1}: {2}", method, url.OriginalString, ex.Message));
                _traceService.Error(ex);
            }

            return response;
        }

        private IHttpActionResult GetResponseBody(RuleModel rule)
        {
            var statuscode = rule.StatusCode;
            string message = "Requested rule {0} {1}. The response has a body with a content type of {2}. The status code is set to {3} ({4}).";
            _traceService.Log(string.Format(message, rule.Method, rule.Url.OriginalString, rule.ContentType, (int)statuscode, statuscode));

            var content = new StringContent(rule.Body, Encoding.UTF8, rule.ContentType);
            var response = new HttpResponseMessage { Content = content, StatusCode = statuscode };

            return ResponseMessage(response);
        }

        private IHttpActionResult GetStatusCode(RuleModel rule)
        {
            var statuscode = rule.StatusCode;
            _traceService.Log(string.Format("Requested rule {0} {1}. The response is a status code of {2} ({3}).", rule.Method, rule.Url.OriginalString, (int)statuscode, statuscode));

            return StatusCode(statuscode);
        }
    }
}
