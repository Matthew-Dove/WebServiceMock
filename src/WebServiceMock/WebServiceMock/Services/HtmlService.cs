using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServiceMock.Models;

namespace WebServiceMock.Services
{
    /// <summary>Contains methods for handling server-side HTML markup.</summary>
    /// <remarks>This should be done using Nancy's Razor engine in a view, but I haven't learn't it yet, this will do for version one.</remarks>
    public interface IHtmlService
    {
        /// <summary>Creates HTML markup for rules to display in a table.</summary>
        /// <param name="rules">A list of rules to display in the table.</param>
        /// <example><tr><td><span>GET</span><a href="/Rule?id=7c9e6679-7425-40de-944b-e07fc1f90ae7">/Users</a></td></tr></example>
        /// <returns>The rows to sit in a table's tbody section.</returns>
        string CreateTableRowsFrom(IEnumerable<RuleViewModel> rules);
    }

    public class HtmlService : IHtmlService
    {
        /// <summary>
        /// The HTML template for a row in a table containing a list of rules.
        /// <para>{0} is replaced by the HTTP verb.</para>
        /// <para>{1} is replaced by rule's Id.</para>
        /// <para>{2} is replaced by the relative mock URL that will return the mocked response.</para>
        /// </summary>
        private const string RULE_ROW_TEMPLATE = "<tr><td><span>{0}</span><a href=\"/Rule?id={1}\">{2}</a></td></tr>";

        public string CreateTableRowsFrom(IEnumerable<RuleViewModel> rules)
        {
            var sb = new StringBuilder();

            foreach (var rule in (rules ?? Enumerable.Empty<RuleViewModel>()))
            {
                sb.AppendFormat(RULE_ROW_TEMPLATE, rule.Method, rule.Id, rule.Url);
            }

            return sb.ToString();
        }
    }
}
