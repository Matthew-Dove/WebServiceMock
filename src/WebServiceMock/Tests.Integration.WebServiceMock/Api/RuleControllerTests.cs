using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                Body = "{ \"productId\": \"47\", \"quantity\": \"2\" }",
                ContentType = "application/json; charset=utf-8",
                StatusCode = HttpStatusCode.OK
            };
        }

        #region Create

        [TestMethod]
        public async Task CreateNewRule()
        {
            var rule = GetNewRule();
            rule.Id = Guid.Empty;

            var createRule = await ApiService.PostAsync("/Api/Rules", rule);
            
            Assert.AreEqual(HttpStatusCode.Created, createRule);
        }

        [TestMethod]
        public async Task CreateSameRule_Twice()
        {
            var rule = GetNewRule();
            rule.Id = Guid.Empty;
            rule.Url = "/products/47";

            var createRule = await ApiService.PostAsync("/Api/Rules", rule);
            var createSameRuleAgain = await ApiService.PostAsync("/Api/Rules", rule);

            Assert.AreEqual(HttpStatusCode.Created, createRule);
            Assert.AreEqual(HttpStatusCode.InternalServerError, createSameRuleAgain);
        }

        #endregion

        #region Update

        [TestMethod]
        public async Task UpdateExistingRule()
        {

        }

        [TestMethod]
        public async Task UpdateRule_ThatDoesNotExist()
        {

        }

        #endregion

        #region Delete

        [TestMethod]
        public async Task DeleteExistingRule()
        {

        }

        [TestMethod]
        public async Task DeleteRule_ThatDoesNotExist()
        {

        }

        #endregion
    }
}
