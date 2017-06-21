using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Tests.Integration.WebServiceMock.Models;

namespace Tests.Integration.WebServiceMock.Api
{
    [TestClass]
    public class RuleControllerTests
    {
        private static Rule GetNewRule()
        {
            return new Rule
            {
                Id = Guid.Empty,
                Url = "/orders/1",
                Method = HttpVerbModel.GET,
                Body = JsonConvert.SerializeObject(new RuleBody { ProductId = 47, Quantity = 2 }),
                ContentType = "application/json",
                StatusCode = HttpStatusCode.OK
            };
        }

        #region Create

        [TestMethod]
        public async Task CreateNewRule_Valid()
        {
            var rule = GetNewRule();
            rule.Url = "/orders/2";

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var path = create.Location.AbsolutePath;
            var get = await ApiService.GetAsync<RuleBody>(path.Substring(1, path.Length - 1)); // Make sure the new rule exists.

            Assert.AreEqual(HttpStatusCode.Created, create.StatusCode);
            Assert.AreEqual(rule.StatusCode, get.StatusCode);
            Assert.AreNotEqual(Guid.Empty, create.Response.Id);
            Assert.AreEqual(rule.Url, create.Response.Url);
            Assert.AreEqual(rule.Method, create.Response.Method);
            Assert.AreEqual(rule.Body, create.Response.Body);
            Assert.AreEqual(rule.ContentType, create.Response.ContentType);
            Assert.AreEqual(rule.StatusCode, create.Response.StatusCode);
        }

        [TestMethod]
        public async Task CreateSameRule_Twice_NotValid()
        {
            var rule = GetNewRule();
            rule.Url = "/orders/3";

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var createAgain = await ApiService.PostAsync<Rule, Rule>("api/rules", rule); // Identical url and HttpMethod is not allowed.

            Assert.AreEqual(HttpStatusCode.Created, create.StatusCode);
            Assert.AreEqual(HttpStatusCode.InternalServerError, createAgain.StatusCode);
        }

        #endregion

        #region Update

        [TestMethod]
        public async Task UpdateExistingRule_Valid()
        {
            var rule = GetNewRule();
            rule.Url = "/orders/4";
            var ruleBody = new RuleBody { ProductId = 47, Quantity = 3 };

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var path = create.Location.AbsolutePath;
            rule.Id = create.Response.Id;
            rule.Body = JsonConvert.SerializeObject(ruleBody);
            var update = await ApiService.PutAsync("api/rules", rule);
            var get = await ApiService.GetAsync<RuleBody>(path.Substring(1, path.Length - 1)); // Make sure the body has been updated.

            Assert.AreEqual(HttpStatusCode.NoContent, update);
            Assert.AreEqual(ruleBody.ProductId, get.Response.ProductId);
            Assert.AreEqual(ruleBody.Quantity, get.Response.Quantity);
        }

        [TestMethod]
        public async Task UpdateRule_ThatDoesNotExist_NotValid()
        {
            var rule = GetNewRule();
            rule.Url = "/orders/5";
            rule.Id = Guid.NewGuid(); // New Id ensures this rule won't exist yet.

            var update = await ApiService.PutAsync("api/rules", rule);

            Assert.AreEqual(HttpStatusCode.InternalServerError, update);
        }

        #endregion

        #region Delete

        [TestMethod]
        public async Task DeleteExistingRule_Valid()
        {

        }

        [TestMethod]
        public async Task DeleteRule_ThatDoesNotExist_NotValid()
        {

        }

        #endregion
    }
}
