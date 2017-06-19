using System;
using System.Net;
using System.Web.Http;
using WebServiceMock.Models;
using WebServiceMock.Services;

namespace WebServiceMock.Api
{
    /// <summary>Creates, modifies, and removes rules. Rules are used to describe mock endpoints, and how they should behave.</summary>
    public class RulesController : ApiController
    {
        private readonly IRuleService _ruleService = null;
        private readonly ITraceService _traceService = null;

        public RulesController(IRuleService ruleService, ITraceService traceService)
        {
            _ruleService = ruleService;
            _traceService = traceService;
        }

        /// <summary>Creates a new rule.</summary>
        public IHttpActionResult Post(RuleModel rule)
        {
            if (!RuleModel.IsValid(rule))   return BadRequest("The rule's values are out of range.");
            if (rule.Id != Guid.Empty)      return BadRequest("When creating a rule, the rule cannot have an Id.");

            // I have an unhealthy hate for nulls...
            if (rule.ContentType == null)   rule.ContentType = string.Empty;
            if (rule.Body == null)          rule.Body = string.Empty;

            IHttpActionResult response = InternalServerError();

            try
            {
                rule.Id = _ruleService.UpsertRule(rule);
                response = Created(string.Concat(Request.RequestUri.GetLeftPart(UriPartial.Authority), RuleModel.START_URL, rule.Url.OriginalString), rule);
            }
            catch (Exception ex)
            {
                _traceService.Error(ex);
            }

            return response;
        }

        /// <summary>Updates an existing rule.</summary>
        public IHttpActionResult Put(RuleModel rule)
        {
            if (!RuleModel.IsValid(rule))   return BadRequest("The rule's values are out of range.");
            if (rule.Id == Guid.Empty)      return BadRequest("When updating a rule, the rule must have an Id.");

            if (rule.ContentType == null)   rule.ContentType = string.Empty;
            if (rule.Body == null)          rule.Body = string.Empty;

            IHttpActionResult response = InternalServerError();

            try
            {
                _ruleService.UpsertRule(rule);
                response = StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _traceService.Error(ex);
            }

            return response;
        }

        /// <summary>Removes an existing rule.</summary>
        [Route("api/rules/{ruleId}")]
        public IHttpActionResult Delete(Guid ruleId)
        {
            if (ruleId == Guid.Empty) return BadRequest("When deleting a rule, the rule must have an Id.");

            IHttpActionResult response = InternalServerError();

            try
            {
                _ruleService.DeleteRule(ruleId);
                response = StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _traceService.Error(ex);
            }

            return response;
        }
    }
}
