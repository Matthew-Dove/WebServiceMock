using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceMock.Models;
using WebServiceMock.Services;

namespace WebServiceMock.Modules
{
    /// <summary>Shows a short summary of each rule from the repository, with links to further modify the rule.</summary>
    public class HomeModule : NancyModule
    {
        public HomeModule(IRuleService ruleService, IHtmlService htmlService, ITraceService traceService)
        {
            Get["/"] = _ => {
                var rows = string.Empty;

                try
                {
                    var rules = ruleService.SelectRules().Select(x => new RuleViewModel(x)); // Get all existing rules, and display them in a table.
                    rows = htmlService.CreateTableRowsFrom(rules);
                }
                catch (Exception ex)
                {
                    traceService.Error(ex);
                }

                var model = new { TableRows = rows };
                return View["home", model];
            };
        }
    }
}
