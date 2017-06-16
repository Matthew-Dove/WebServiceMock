using Nancy;
using WebServiceMock.Models;

namespace WebServiceMock.Modules
{
    /// <summary>This page presents a UI to create a new rule, the view is the same as the rule update page, but the view model has different values.</summary>
    public class AddRuleModule : NancyModule
    {
        public AddRuleModule()
        {
            Get["/AddRule"] = _ => {
                var model = new UpsertRuleViewModel();
                return View["Rule", model];
            };
        }
    }
}
