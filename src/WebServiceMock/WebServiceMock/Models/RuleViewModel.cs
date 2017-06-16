using System;

namespace WebServiceMock.Models
{
    /// <summary>A summary of a Rule to display on the home view.</summary>
    public class RuleViewModel
    {
        /// <summary>The Id of this rule.</summary>
        public Guid Id { get; set; }

        /// <summary>The url relative from the mock url that this rule can be requested from.</summary>
        public Uri Url { get; set; }

        /// <summary>The HTTP method used when requesting this rule.</summary>
        public HttpVerb Method { get; set; }

        public RuleViewModel()
        {
            Id = Guid.Empty;
            Url = new Uri("/", UriKind.Relative);
            Method = HttpVerb.GET;
        }

        public RuleViewModel(RuleModel rule)
        {
            if (!RuleModel.IsValid(rule))
                throw new ArgumentException("The rule's properties don't have valid values.", "rule");

            Id = rule.Id;
            Url = rule.Url;
            Method = rule.Method;
        }

        public override string ToString()
        {
            return Url == null ? null : Url.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RuleViewModel);
        }

        public bool Equals(RuleViewModel rule)
        {
            return rule != null && rule.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
