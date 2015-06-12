using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServiceMock.Services;
using System.Collections.Generic;
using WebServiceMock.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tests.WebServiceMock.Services
{
    [TestClass]
    public class HtmlServiceTests
    {
        private HtmlService _htmlSevice = null;

        [TestInitialize]
        public void Initialize()
        {
            _htmlSevice = new HtmlService();
        }

        [TestMethod]
        public void CreateTableRowsFrom_RulesAreNull_ReturnsEmptyString()
        {
            IEnumerable<RuleViewModel> rules = null;

            string markup = _htmlSevice.CreateTableRowsFrom(rules);

            Assert.AreEqual(string.Empty, markup);
        }

        [TestMethod]
        public void CreateTableRowsFrom_RulesAreEmpty_ReturnsEmptyString()
        {
            IEnumerable<RuleViewModel> rules = Enumerable.Empty<RuleViewModel>();

            string markup = _htmlSevice.CreateTableRowsFrom(rules);

            Assert.AreEqual(string.Empty, markup);
        }

        [TestMethod]
        public void CreateTableRowsFrom_OneRule_OneRowIsCreated()
        {
            IEnumerable<RuleViewModel> rules = Enumerable.Repeat(new RuleViewModel { Id = Guid.NewGuid(), Method = HttpVerb.GET, Url = new Uri("/Products", UriKind.Relative) }, 1);

            string markup = _htmlSevice.CreateTableRowsFrom(rules);

            // Since we can't fully test what the result would be without implementing the method in the test, counting the table rows will suffice.
            Assert.AreEqual(Regex.Matches(markup, "<tr>").Count, rules.Count());

            Assert.IsTrue(markup.Contains(rules.First().Id.ToString()));
            Assert.IsTrue(markup.Contains(rules.First().Method.ToString()));
            Assert.IsTrue(markup.Contains(rules.First().Url.ToString()));
        }

        [TestMethod]
        public void CreateTableRowsFrom_ThreeRules_ThreeRowAreCreated()
        {
            IEnumerable<RuleViewModel> rules = new RuleViewModel[]
            {
                new RuleViewModel { Id = Guid.NewGuid(), Method = HttpVerb.POST, Url = new Uri("/Users", UriKind.Relative) },
                new RuleViewModel { Id = Guid.NewGuid(), Method = HttpVerb.GET, Url = new Uri("/Users/1", UriKind.Relative) },
                new RuleViewModel { Id = Guid.NewGuid(), Method = HttpVerb.DELETE, Url = new Uri("/Users/1", UriKind.Relative) }
            };

            string markup = _htmlSevice.CreateTableRowsFrom(rules);

            Assert.AreEqual(Regex.Matches(markup, "<tr>").Count, rules.Count());

            foreach (var rule in rules)
            {
                Assert.IsTrue(markup.Contains(rule.Id.ToString()));
                Assert.IsTrue(markup.Contains(rule.Method.ToString()));
                Assert.IsTrue(markup.Contains(rule.Url.ToString()));
            }
        }
    }
}
