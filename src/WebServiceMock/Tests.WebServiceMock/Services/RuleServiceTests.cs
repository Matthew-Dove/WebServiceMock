using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServiceMock.Core.Services;
using Moq;
using WebServiceMock.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tests.WebServiceMock.Services
{
    [TestClass]
    public class RuleServiceTests
    {
        private Mock<IRepositoryService> _repositoryService = null;

        [TestInitialize]
        public void Initialize()
        {
            _repositoryService = new Mock<IRepositoryService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            RuleService.ClearRules();
        }

        #region SELECT

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void SelectRule_RuleWithNoId_ThrowsException()
        {
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = Guid.NewGuid() }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var id = Guid.Empty;

            RuleModel rule = ruleService.SelectRule(id);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void SelectRule_RuleDoesNotExist_ThrowsException()
        {
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = Guid.NewGuid() }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var id = Guid.NewGuid();

            RuleModel rule = ruleService.SelectRule(id);
        }

        [TestMethod]
        public void SelectRule_OneRuleSaved_GetSpecificRule()
        {
            var id = Guid.NewGuid();
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = id, Url = new Uri("/Users", UriKind.Relative) }, 1));
            var ruleService = new RuleService(_repositoryService.Object);

            RuleModel rule = ruleService.SelectRule(id);

            Assert.AreEqual(id, rule.Id);
        }

        [TestMethod]
        public void SelectRule_ThreeRulesSaved_GetSpecificRule()
        {
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Users", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);
            var ruleService = new RuleService(_repositoryService.Object);

            RuleModel rule = ruleService.SelectRule(repositoryRules[0].Id);

            Assert.AreEqual(repositoryRules[0].Id, rule.Id);
        }

        [TestMethod]
        public void SelectRules_NoRulesSaved_GetEmptyList()
        {
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Empty<RuleModel>());
            var ruleService = new RuleService(_repositoryService.Object);

            IEnumerable<RuleModel> rules = ruleService.SelectRules();
            
            Assert.AreEqual(0, rules.Count());
        }

        [TestMethod]
        public void SelectRules_OneRulesSaved_GetOneRule()
        {
            var id = Guid.NewGuid();
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = id, Url = new Uri("/Login", UriKind.Relative) }, 1));
            var ruleService = new RuleService(_repositoryService.Object);

            IEnumerable<RuleModel> rules = ruleService.SelectRules();

            Assert.AreEqual(1, rules.Count());
            Assert.AreEqual(id, rules.First().Id);
        }

        [TestMethod]
        public void SelectRules_ThreeRulesSaved_GetThreeRules()
        {
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Users", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);
            var ruleService = new RuleService(_repositoryService.Object);

            IEnumerable<RuleModel> rules = ruleService.SelectRules();

            Assert.AreEqual(repositoryRules.Length, rules.Count());

            for (int i = 0; i < repositoryRules.Length; i++)
            {
                Assert.AreEqual(repositoryRules[i].Id, rules.ElementAt(i).Id);
            }
        }

        #endregion

        #region UPSERT

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void UpsertRule_RuleIsNull_ThrowsException()
        {
            var ruleService = new RuleService(_repositoryService.Object);
            RuleModel rule = null;

            ruleService.UpsertRule(rule);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void UpsertRule_RuleHasIdAndListHasNoMatch_ThrowsException()
        {
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = Guid.NewGuid() }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var rule = new RuleModel { Id = Guid.NewGuid() };

            ruleService.UpsertRule(rule);
        }

        [TestMethod]
        public void UpsertRule_RuleHasIdAndListHasMatchingId_RuleIsUpdated()
        {
            var id = Guid.NewGuid();
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = id, Url = new Uri("/Login", UriKind.Relative) }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var relativeUrl = "/UpsertRule";
            var rule = new RuleModel { Id = id, Url = new Uri(relativeUrl, UriKind.Relative) };

            ruleService.UpsertRule(rule);

            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Single().Id == id)), Times.Once);
            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Single().Url.ToString() == relativeUrl)), Times.Once);
        }

        [TestMethod]
        public void UpsertRule_RuleHasIdAndListWithOtherRulesHasMatchingId_RuleIsUpdated()
        {
            var id = Guid.NewGuid();
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = id, Url = new Uri("/Users", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);
            var ruleService = new RuleService(_repositoryService.Object);
            var relativeUrl = "/UpsertRule";
            var rule = new RuleModel { Id = id, Url = new Uri(relativeUrl, UriKind.Relative) };

            ruleService.UpsertRule(rule);

            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Count() == repositoryRules.Length)), Times.Once);
            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Single(z => z.Id == id).Url.ToString() == relativeUrl)), Times.Once);
        }

        [TestMethod]
        public void UpsertRule_RuleHasNoId_RuleIsInsertedWithAnId()
        {
            var ruleService = new RuleService(_repositoryService.Object);
            var relativeUrl = "/UpsertRule";
            var rule = new RuleModel { Url = new Uri(relativeUrl, UriKind.Relative) };

            ruleService.UpsertRule(rule);

            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Single().Id != Guid.Empty)), Times.Once);
            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Single().Url.ToString() == relativeUrl)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void UpsertRule_InsertRuleUrlAlreadyExists_ThrowsException()
        {
            string url = "/users";
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat<RuleModel>(new RuleModel { Id = Guid.NewGuid(), Url = new Uri(url, UriKind.Relative) }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var rule = new RuleModel { Url = new Uri(url, UriKind.Relative) };

            ruleService.UpsertRule(rule);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void UpsertRule_UpdateRuleUrlAlreadyExists_ThrowsException()
        {
            string url = "/users";
            var id = Guid.NewGuid();
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri(url, UriKind.Relative) },
                new RuleModel { Id = id, Url = new Uri("/Products", UriKind.Relative) }
            };

            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);
            var ruleService = new RuleService(_repositoryService.Object);
            var rule = new RuleModel { Id = id, Url = new Uri(url, UriKind.Relative) };

            ruleService.UpsertRule(rule);
        }

        [TestMethod]
        public void UpsertRule_UpdateTheSameRuleWithTheRuleWithSameUrl_UpdatesRule()
        {
            string url = "/users";
            var id = Guid.NewGuid();

            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = id, Url = new Uri(url, UriKind.Relative) }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var rule = new RuleModel { Id = id, Url = new Uri(url, UriKind.Relative) };

            ruleService.UpsertRule(rule);
        }

        [TestMethod]
        public void UpsertRule_RuleHasNoIdListHasOtherRules_RuleIsInsertedWithAnId()
        {
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Users", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);
            var ruleService = new RuleService(_repositoryService.Object);
            var relativeUrl = "/UpsertRule";
            var rule = new RuleModel { Url = new Uri(relativeUrl, UriKind.Relative) };

            ruleService.UpsertRule(rule);

            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Count(z => z.Id == Guid.Empty) == 0)), Times.Once);
            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Count(z => z.Url.ToString() == relativeUrl) == 1)), Times.Once);
            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Count() == repositoryRules.Length + 1)), Times.Once);
        }

        [TestMethod]
        public void UpsertRule_InsertRule_GetNewId()
        {
            var ruleService = new RuleService(_repositoryService.Object);
            var rule = new RuleModel { Url = new Uri("/users", UriKind.Relative) };

            Guid ruleId = ruleService.UpsertRule(rule);

            Assert.AreNotEqual(Guid.Empty, ruleId);
        }

        [TestMethod]
        public void UpsertRule_UpdateRule_GetTheSameId()
        {
            var id = Guid.NewGuid();
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = id, Url = new Uri("/users", UriKind.Relative) }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var rule = new RuleModel { Id = id, Url = new Uri("/products", UriKind.Relative) };

            Guid ruleId = ruleService.UpsertRule(rule);

            Assert.AreEqual(id, ruleId);
        }

        #endregion

        #region DELETE

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void DeleteRule_RuleWithNoId_ThrowsException()
        {
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = Guid.NewGuid() }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var id = Guid.Empty;

            ruleService.DeleteRule(id);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void DeleteRule_RuleDoesNotExist_ThrowsException()
        {
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = Guid.NewGuid() }, 1));
            var ruleService = new RuleService(_repositoryService.Object);
            var id = Guid.NewGuid();

            ruleService.DeleteRule(id);
        }

        [TestMethod]
        public void DeleteRule_DeleteLastRuleInList_UpdatesRepositoryWithEmptyList()
        {
            var id = Guid.NewGuid();
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Repeat(new RuleModel { Id = id, Url = new Uri("/users", UriKind.Relative) }, 1));
            var ruleService = new RuleService(_repositoryService.Object);

            ruleService.DeleteRule(id);
            
            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Count() == 0)), Times.Once);
        }

        [TestMethod]
        public void DeleteRule_DeleteRuleInListOfThree_UpdatesRepositoryWithTwoLeftInList()
        {
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Users", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);
            var ruleService = new RuleService(_repositoryService.Object);
            var id = repositoryRules[1].Id;

            ruleService.DeleteRule(id);

            _repositoryService.Verify(x => x.SaveRules(It.Is<IEnumerable<RuleModel>>(y => y.Count(z => z.Id == id) == 0 && y.Count() == 2)), Times.Once);
        }

        #endregion

        #region Search

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void TryGetRule_NullUrl_ThrowsException()
        {
            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = null;
            RuleModel rule;

            ruleService.TryGetRule(url, HttpVerb.GET, out rule);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void TryGetRule_UrlTooShort_ThrowsException()
        {
            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = new Uri("/");
            RuleModel rule;

            ruleService.TryGetRule(url, HttpVerb.GET, out rule);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void TryGetRule_UrlNotRelative_ThrowsException()
        {
            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = new Uri("https://api.com/");
            RuleModel rule;

            ruleService.TryGetRule(url, HttpVerb.GET, out rule);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void TryGetRule_MethodNotvalidValue_ThrowsException()
        {
            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = new Uri("/users", UriKind.Relative);
            RuleModel rule;

            ruleService.TryGetRule(url, HttpVerb.GET & HttpVerb.POST, out rule);
        }

        [TestMethod]
        public void TryGetRule_EmptyRepository_ReturnsFalseAndNoRule()
        {
            _repositoryService.Setup(x => x.GetRules()).Returns(Enumerable.Empty<RuleModel>());

            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = new Uri("/users", UriKind.Relative);
            RuleModel rule;

            bool found = ruleService.TryGetRule(url, HttpVerb.GET, out rule);

            Assert.IsFalse(found);
            Assert.IsNull(rule);
        }

        [TestMethod]
        public void TryGetRule_UrlDoesNotExist_ReturnsFalseAndNoRule()
        {
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Users", UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);

            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = new Uri("/Tokens", UriKind.Relative);
            RuleModel rule;

            bool found = ruleService.TryGetRule(url, HttpVerb.GET, out rule);

            Assert.IsFalse(found);
            Assert.IsNull(rule);
        }

        [TestMethod]
        public void TryGetRule_UrlExistsWithDifferentMethod_ReturnsFalseAndNoRule()
        {
            var id = Guid.NewGuid();
            var usersUrl = "/users";
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = id, Url = new Uri(usersUrl, UriKind.Relative), Method = HttpVerb.GET },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);

            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = new Uri(usersUrl, UriKind.Relative);
            RuleModel rule;

            bool found = ruleService.TryGetRule(url, HttpVerb.POST, out rule);

            Assert.IsFalse(found);
            Assert.IsNull(rule);
        }

        [TestMethod]
        public void TryGetRule_UrlExistsWithDifferentCasing_ReturnsTrueAndTheRule()
        {
            var id = Guid.NewGuid();
            string usersUrl = "/users";
            var repositoryRules = new RuleModel[]
            {
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Login", UriKind.Relative) },
                new RuleModel { Id = id, Url = new Uri(usersUrl.ToUpper(), UriKind.Relative) },
                new RuleModel { Id = Guid.NewGuid(), Url = new Uri("/Products", UriKind.Relative) }
            };
            _repositoryService.Setup(x => x.GetRules()).Returns(repositoryRules);

            var ruleService = new RuleService(_repositoryService.Object);
            Uri url = new Uri(usersUrl.ToLower(), UriKind.Relative);
            RuleModel rule;

            bool found = ruleService.TryGetRule(url, HttpVerb.GET, out rule);

            Assert.IsTrue(found);
            Assert.AreEqual(id, rule.Id);
        }

        #endregion
    }
}
