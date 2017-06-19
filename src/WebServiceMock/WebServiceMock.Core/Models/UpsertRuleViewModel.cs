using Nancy.Helpers;
using System;
using System.Net;

namespace WebServiceMock.Core.Models
{
    /// <summary>A view model for a rule, containing the required fields to upsert a rule.</summary>
    public class UpsertRuleViewModel
    {
        /// <summary>The relative url that goes before all custom urls from rules.</summary>
        public string StartUrl { get { return RuleModel.START_URL; } }

        /// <summary>The rule's Id.</summary>
        public Guid Id { get; set; }

        /// <summary>The rule's relative url that will activate the rule's response.</summary>
        public string Url { get; set; }

        /// <summary>The HTTP method that activates the rule along with the URL.</summary>
        public HttpVerb Method { get; set; }

        /// <summary>The HTTP status code to return as part of the rule's response.</summary>
        public int StatusCode { get; set; }

        /// <summary>The rule's response body's content type (the body is optional).</summary>
        public string ContentType { get; set; }

        /// <summary>The text to return as the resposne body (the body is optional).</summary>
        public string Body { get; set; }

        /// <summary>The HTTP method to send to the server when saving the rule's values.</summary>
        public HttpVerb RequestType { get; set; }

        /// <summary>The text to display to the user on the save button.</summary>
        public string SubmitMessage { get; set; }

        /// <summary>Contains a CSS class to hide the delete button if the rule is being created (not updated).</summary>
        public string DisplayDelete { get; set; }

        /// <summary>Creates the model in an insert state.</summary>
        public UpsertRuleViewModel()
        {
            Id = Guid.Empty;
            Url = string.Empty;
            Method = HttpVerb.GET;
            StatusCode = (int)HttpStatusCode.OK;
            ContentType = string.Empty;
            Body = string.Empty;
            RequestType = HttpVerb.POST;
            SubmitMessage = "Create Rule";
            DisplayDelete = "hide";
        }

        /// <summary>Creates the model in an update state.</summary>
        public UpsertRuleViewModel(RuleModel rule)
        {
            if (!RuleModel.IsValid(rule))
                throw new ArgumentException("The rule's values aren't in a valid state.", "rule");

            Id = rule.Id;
            Url = HttpUtility.HtmlEncode(rule.Url.OriginalString);
            Method = rule.Method;
            StatusCode = (int)rule.StatusCode;
            ContentType = HttpUtility.HtmlEncode(rule.ContentType);
            Body = HttpUtility.HtmlEncode(rule.Body);
            RequestType = HttpVerb.PUT;
            SubmitMessage = "Update Rule";
            DisplayDelete = string.Empty;
        }
    }
}
