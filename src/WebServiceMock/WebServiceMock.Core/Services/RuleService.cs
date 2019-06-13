using System;
using System.Collections.Generic;
using System.Linq;
using WebServiceMock.Core.Models;

namespace WebServiceMock.Core.Services
{
    /// <summary>Handles all data operations on rules.</summary>
    public interface IRuleService
    {
        /// <summary>Gets a single rule.</summary>
        /// <param name="id">The rule's id to select.</param>
        /// <returns>The Rule's model for the specified Id.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the id is Guid.Empty, or the id doesn't exist.</exception>
        RuleModel SelectRule(Guid id);

        /// <summary>Gets all the rules created by the client, this list can be empty.</summary>
        /// <returns>The list of created rules.</returns>
        IEnumerable<RuleModel> SelectRules();

        /// <summary>
        /// Inserts. or updates a rule.
        /// <para>If the rule's Id equals Guid.Empty, an insert occurs.</para>
        /// <para>Otherwise the rule is updated, if the Id doesn't exist, an exception is thrown.</para>
        /// </summary>
        /// <param name="rule">The rule to upsert.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the rule is in an invalid state, or the rule's url is already in use by another rule.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the id isn't Guid.Empty (an update), and the id doesn't exist.</exception>
        /// <returns>The Id of the rule that was just upserted.</returns>
        Guid UpsertRule(RuleModel rule);

        /// <summary>Removes a rule from the repository.</summary>
        /// <param name="id">The rule's Id.</param>
        /// <exception cref="System.ArgumentException">Thrown when the id is Guid.Empty, or the id doesn't exist.</exception>
        void DeleteRule(Guid id);

        /// <summary>Gets a rule if the url has a matching rule associated with it.</summary>
        /// <param name="url">The relative url to search for (after the mock endpoint).</param>
        /// <param name="method">The HTTP method used when requesting the URL.</param>
        /// <param name="rule">The associated rule for the url, or null if not found.</param>
        /// <exception cref="System.ArgumentNullException">When the url is null.</exception>
        /// <exception cref="System.ArgumentException">When the url is not in the expected format.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">When the method isn't a valid enum value.</exception>
        /// <returns>True if the url has a corresponding rule.</returns>
        bool TryGetRule(Uri url, HttpVerb method, out RuleModel rule);
    }

    /// <summary>
    /// Instead of being held in memory backed by a textfile, these rules should read from a local database.
    /// <para>Version one this will do, no need to web scale an app running for a single machine.</para>
    /// </summary>
    public class RuleService : IRuleService
    {
        private static readonly object _lock = new object();
        private static RuleModel[] _rules = null;
        private static Dictionary<Guid, int> _ruleIdMappings = null;
        private static Dictionary<string, int> _ruleUrlMappings = null;

        private readonly IRepositoryService _repositoryService = null;

        public RuleService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public RuleModel SelectRule(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("id", "id cannot be Guid.Empty.");

            return GetRuleBy(id);
        }

        public IEnumerable<RuleModel> SelectRules()
        {
            return GetCopyOfRules();
        }

        public Guid UpsertRule(RuleModel rule)
        {
            if (!RuleModel.IsValid(rule))
                throw new ArgumentOutOfRangeException("rule");

            Guid id;

            if (rule.Id == Guid.Empty)
            {
                id = InsertRule(rule);
            }
            else
            {
                id = UpdateRule(rule);
            }

            return id;
        }

        public void DeleteRule(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("id", "id cannot be Guid.Empty.");

            RemoveRuleBy(id);
        }

        public bool TryGetRule(Uri url, HttpVerb method, out RuleModel rule)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (url.IsAbsoluteUri)
                throw new ArgumentException("The url must be relative.", "url");
            if (url.OriginalString.Length < 2)
                throw new ArgumentException("The url have at least two characters.", "url");
            if (url.OriginalString[0] != '/')
                throw new ArgumentException("The url must start with a forward slash (/).", "url");
            if (!Enum.IsDefined(typeof(HttpVerb), method))
                throw new ArgumentOutOfRangeException("Method");

            rule = GetRuleBy(url, method);
            return rule != null;
        }

        /// <summary>Finds the rule by its url, if the id has no match, null is returned.</summary>
        private RuleModel GetRuleBy(Uri url, HttpVerb method)
        {
            RuleModel rule = null;

            lock (_lock)
            {
                var rules = GetRules();
                int ruleIndex;

                if (_ruleUrlMappings.TryGetValue(string.Concat((int)method, url.OriginalString.ToLower()), out ruleIndex))
                {
                    rule = rules[ruleIndex];
                }
            }

            return rule;
        }

        /// <summary> Removes the rule by its id, if the id has no match, an exception is thrown.</summary>
        private void RemoveRuleBy(Guid id)
        {
            lock (_lock)
            {
                var rule = GetRuleBy(id); // Make sure the rule exists.
                var ruleIndex = _ruleIdMappings[rule.Id];
                var newRules = new RuleModel[_rules.Length - 1];

                for (int i = 0, offset = 0; i < newRules.Length; i++, offset++)
                {
                    if (i == ruleIndex)
                    {
                        offset++; // Skip the element marked for to deletion.
                    }

                    newRules[i] = _rules[offset];
                }

                _repositoryService.SaveRules(newRules);
                ClearRules(); // Force the rules to read from the repository to ensure they're insync.
            }
        }

        /// <summary>Adds a new rule to the rule array.</summary>
        private Guid InsertRule(RuleModel rule)
        {
            rule.Id = Guid.NewGuid();
            var newRule = new RuleModel(rule);

            lock (_lock)
            {
                var rules = GetRules();
                if (GetRuleBy(rule.Url, rule.Method) != null)
                {
                    throw new ArgumentOutOfRangeException("rule", string.Format("Cannot insert the rule, as the url {0} is in use by another rule.", rule.Url));
                }

                Array.Resize(ref rules, rules.Length + 1);
                rules[rules.Length - 1] = newRule;
                _repositoryService.SaveRules(rules);
                ClearRules(); // Force the rules to read from the repository to ensure they're insync.
            }

            return rule.Id;
        }

        /// <summary>Replaces the existing rule with the new one.</summary>
        private Guid UpdateRule(RuleModel rule)
        {
            lock (_lock)
            {
                var existingRule = GetRuleBy(rule.Id); // Make sure the rule exists.
                var ruleByUrl = GetRuleBy(rule.Url, rule.Method); // Make sure the url isn't already in use.

                if (ruleByUrl != null && ruleByUrl.Id != existingRule.Id)
                {
                    throw new ArgumentOutOfRangeException("rule", string.Format("Cannot update the rule, as the url {0} is in use by another rule.", existingRule));
                }

                var ruleIndex = _ruleIdMappings[existingRule.Id];

                _rules[ruleIndex] = new RuleModel(rule); // Replace old rule with new rule.
                _repositoryService.SaveRules(_rules);
                ClearRules(); // Force the rules to read from the repository to ensure they're insync.
            }

            return rule.Id;
        }

        /// <summary>Finds the rule by its id, if the id has no match, an exception is thrown.</summary>
        private RuleModel GetRuleBy(Guid id)
        {
            lock (_lock)
            {
                int ruleIndex;
                var rules = GetRules();

                if (_ruleIdMappings.TryGetValue(id, out ruleIndex))
                {
                    return rules[ruleIndex];
                }
            }

            throw new ArgumentException("id", string.Format("The id {0} doesn't match any existing rule.", id));
        }

        /// <summary>Only a shallow copy (the referenced objects by both arrays stay the same, but value types are different copies).</summary>
        private RuleModel[] GetCopyOfRules()
        {
            RuleModel[] copy = null;

            lock(_lock)
            {
                copy = (RuleModel[])GetRules().Clone();
            }

            return copy;
        }

        /// <summary>Gets the rules in their current state, if the rules have no value, the rules are read from the repository.</summary>
        private RuleModel[] GetRules()
        {
            if (_rules == null)
            {
                lock (_lock)
                {
                    if (_rules == null)
                    {
                        _rules = _repositoryService.GetRules().ToArray();
                        _ruleIdMappings = new Dictionary<Guid, int>(_rules.Length);
                        _ruleUrlMappings = new Dictionary<string, int>(_rules.Length);

                        for (int i = 0; i < _rules.Length; i++)
                        {
                            _ruleIdMappings.Add(_rules[i].Id, i);
                            _ruleUrlMappings.Add(string.Concat((int)_rules[i].Method, _rules[i].Url.OriginalString.ToLower()), i); 
                        }
                    }
                }
            }

            return _rules;
        }

        /// <summary>Unit tests need to reset this object on each run.</summary>
        public static void ClearRules()
        {
            lock (_lock)
            {
                _rules = null;
            }
        }
    }
}
