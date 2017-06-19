using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using WebServiceMock.Core.Models;

namespace WebServiceMock.Core.Services
{
    /// <summary>Stores and loads data across requests.</summary>
    public interface IRepositoryService
    {
        /// <summary>Gets all saved rules, the rules can be empty.</summary>
        /// <returns>The rules previously saved to the repository.</returns>
        IEnumerable<RuleModel> GetRules();

        /// <summary>Saves all rules, the rules can be empty.</summary>
        /// <param name="rules">The rules to save.</param>
        void SaveRules(IEnumerable<RuleModel> rules);
    }

    /// <summary>No need to unit test this (no business logic), only integration testing.</summary>
    public class RepositoryService : IRepositoryService
    {
        private readonly string _path = null;

        public RepositoryService()
        {
            _path = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "rules.txt");
        }

        public IEnumerable<RuleModel> GetRules()
        {
            CreateFileIfMissing();

            var json = File.ReadAllText(_path);
            var rules = JsonConvert.DeserializeObject<IEnumerable<RuleModel>>(json);

            return rules;
        }

        public void SaveRules(IEnumerable<RuleModel> rules)
        {
            if (rules == null)
                throw new ArgumentNullException("rules");

            CreateFileIfMissing();

            var json = JsonConvert.SerializeObject(rules, Formatting.Indented);
            File.WriteAllText(_path, json);
        }

        /// <summary>Checks for the rules file, and creates it if it doesn't exist.</summary>
        private void CreateFileIfMissing()
        {
            if (!File.Exists(_path))
            {
                File.WriteAllText(_path, "[]");
            }
        }
    }
}
