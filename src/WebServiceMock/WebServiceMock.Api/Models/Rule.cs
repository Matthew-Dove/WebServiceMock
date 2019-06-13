using System;

namespace WebServiceMock.Api.Models
{
    /// <summary>>Describes the parameters to match an incoming rule when externally requested.</summary>
    public struct Rule
    {
        /// <summary>The HTTP method used when requesting this rule.</summary>
        public Verb Method { get; }

        /// <summary>The relative path from ~/api/mock used when requesting this rule, must start with a forward slash (/).</summary>
        public Uri Path { get; }

        /// <summary>Describes the parameters to match an incoming rule when externally requested.</summary>
        /// <param name="method">The HTTP method used when requesting this rule.</param>
        /// <param name="path">The relative path from ~/api/mock used when requesting this rule, must start with a forward slash (/).</param>
        public Rule(Verb method, Uri path)
        {
            if (!Enum.IsDefined(typeof(Verb), method))
                throw new ArgumentException("The HTTP method is not a defined value.", nameof(method));
            if (path == null)
                throw new ArgumentNullException(nameof(path), "The relative uri is required.");

            Method = method;
            Path = path;
        }
    }
}
