using Nancy;
using Nancy.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceMock.Models;
using WebServiceMock.Services;

namespace WebServiceMock.Modules
{
    /// <summary>This page presents a UI to update an existing rule, the view is the same as the rule create page, but the view model has different values.</summary>
    public class RuleModule : NancyModule
    {
        public RuleModule(ITraceService traceService, IRuleService ruleService)
        {
            Get["/Rule"] = _ => {
                dynamic response = Response.AsRedirect("/", Nancy.Responses.RedirectResponse.RedirectType.Temporary); // If the id is missing or invalid, redirect the client home.
                var keyValues = HttpUtility.ParseQueryString(Request.Url.Query);
                var id = keyValues["id"];
                Guid ruleId;

                if (Guid.TryParse(id, out ruleId))
                {
                    try
                    {
                        var rule = ruleService.SelectRule(ruleId);
                        var model = new UpsertRuleViewModel(rule); // Get the existing rule from the id, and map it's values to the view model's properties.
                        response = View["Rule", model];
                    }
                    catch (Exception ex)
                    {
                        traceService.Error(ex);
                    }
                }
                else
                {
                    traceService.Error(string.Concat("The id ", id, " is not a valid rule id."));
                }

                return response;
            };
        }
    }
}
