using System;
using System.Net;

namespace WebServiceMock.Core.Models
{
    /// <summary>Represents a rule, consisting of a relative url and a HTTP method that should return a mock response when requested.</summary>
    public class RuleModel
    {
        /// <summary>The url relative from the host name and port number, that rule mock endpoints must start with.</summary>
        public const string START_URL = "/Api/Mock";

        /// <summary>The Id of this rule.</summary>
        public Guid Id { get; set; }

        /// <summary>The url relative from the mock url that this rule can be requested from.</summary>
        public Uri Url { get; set; }

        /// <summary>The HTTP method used when requesting this rule.</summary>
        public HttpVerb Method { get; set; }

        /// <summary>The status code to return when the mock URL is requested.</summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The content type to return with the response body.
        /// <para>This is optional, and can be empty.</para>
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The body of the response to return when the mock URL is requested.
        /// <para>This is optional, and can be empty.</para>
        /// </summary>
        public string Body { get; set; }

        /// <summary>Checks if this rule should return a response body, as well as a status code.</summary>
        /// <returns>True if both the content type, and the response body have values.</returns>
        public bool HasResponseBody { get { return !string.IsNullOrEmpty(ContentType) && !string.IsNullOrEmpty(Body); } }

        public RuleModel()
        {
            Id = Guid.Empty;
            Url = new Uri("/", UriKind.Relative);
            Method = HttpVerb.GET;
            StatusCode = HttpStatusCode.OK;
            ContentType = string.Empty;
            Body = string.Empty;
        }

        public RuleModel(RuleModel rule)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");

            Id = rule.Id;
            Url = rule.Url;
            Method = rule.Method;
            StatusCode = rule.StatusCode;
            ContentType = rule.ContentType;
            Body = rule.Body;
        }

        /// <summary>Checks if a rule's properties are in a valid range of possible ranges.</summary>
        /// <param name="rule">The rule to test.</param>
        /// <remarks>The Id isn't checked, as Guid.Empty is valid for new rules, and a Guid is valid for existing Ids.</remarks>
        /// <returns>True of the rule is in a valid state.</returns>
        public static bool IsValid(RuleModel rule)
        {
            bool isValid = rule != null && rule.Url != null;
            string url = isValid ? rule.Url.OriginalString : null;

            isValid = isValid && !rule.Url.IsAbsoluteUri;
            isValid = isValid && url.Length > 1 && url.Length < 2000; // The > 1 is for a forward slash and a character (e.g. /a) the minimum allowed url, < 2000 is chossen arbitrarily.
            isValid = isValid && url[0] == '/';
            isValid = isValid && Enum.IsDefined(typeof(HttpVerb), rule.Method);
            isValid = isValid && Enum.IsDefined(typeof(HttpStatusCode), rule.StatusCode);

            // If the Body has a value, so must the ContentType (and vise-versa), otherwise they can both be null (or empty).
            isValid = isValid && string.IsNullOrEmpty(rule.Body) == string.IsNullOrEmpty(rule.ContentType);

            return isValid;
        }

        public override string ToString()
        {
            return Url == null ? null : Url.OriginalString;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RuleModel);
        }

        public bool Equals(RuleModel rule)
        {
            return rule != null && rule.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
