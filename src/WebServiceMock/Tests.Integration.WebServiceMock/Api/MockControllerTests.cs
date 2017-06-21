using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Tests.Integration.WebServiceMock.Models;

namespace Tests.Integration.WebServiceMock.Api
{
    [TestClass]
    public class MockControllerTests
    {
        private static Rule GetNewRule()
        {
            return new Rule
            {
                Id = Guid.Empty,
                Url = "/products/1",
                Method = HttpVerbModel.GET,
                Body = JsonConvert.SerializeObject(new RuleBody { ProductId = 64, Quantity = 8 }),
                ContentType = "application/json",
                StatusCode = HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task Get()
        {
            var rule = GetNewRule();
            rule.Url = "/products/2";
            rule.Method = HttpVerbModel.GET;

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var path = create.Location.AbsolutePath;
            var get = await ApiService.GetAsync<RuleBody>(path.Substring(1, path.Length - 1));

            Assert.AreEqual(rule.StatusCode, get.StatusCode);
        }

        [TestMethod]
        public async Task Post()
        {
            var rule = GetNewRule();
            rule.Url = "/products/3";
            rule.Method = HttpVerbModel.POST;

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var path = create.Location.AbsolutePath;
            var post = await ApiService.PostAsync<string, RuleBody>(path.Substring(1, path.Length - 1), string.Empty); // The body can be any value.

            Assert.AreEqual(rule.StatusCode, post.StatusCode);
        }

        [TestMethod]
        public async Task Put()
        {
            var rule = GetNewRule();
            rule.Url = "/products/4";
            rule.Method = HttpVerbModel.PUT;

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var path = create.Location.AbsolutePath;
            var put = await ApiService.PutAsync(path.Substring(1, path.Length - 1), string.Empty); // The body can be any value.

            Assert.AreEqual(rule.StatusCode, put);
        }

        [TestMethod]
        public async Task Delete()
        {
            var rule = GetNewRule();
            rule.Url = "/products/5";
            rule.Method = HttpVerbModel.DELETE;

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var path = create.Location.AbsolutePath;
            var delete = await ApiService.DeleteAsync(path.Substring(1, path.Length - 1));

            Assert.AreEqual(rule.StatusCode, delete);
        }

        [TestMethod]
        public async Task DoesNotExist_NotFound()
        {
            var path = "api/mock/products/6";

            var get = await ApiService.GetAsync<RuleBody>(path);

            Assert.AreEqual(HttpStatusCode.NotFound, get.StatusCode);
        }

        [TestMethod]
        public async Task Rule_CreatedRule_And_MockedRule_AreEqual()
        {
            var ruleBody = new RuleBody { ProductId = 128, Quantity = 16 };
            var rule = GetNewRule();
            rule.Url = "/products/7";
            rule.Method = HttpVerbModel.GET;
            rule.Body = JsonConvert.SerializeObject(ruleBody);

            var create = await ApiService.PostAsync<Rule, Rule>("api/rules", rule);
            var path = create.Location.AbsolutePath;
            var get = await ApiService.GetAsync<RuleBody>(path.Substring(1, path.Length - 1));

            Assert.AreEqual(rule.Body, create.Response.Body);
            Assert.AreEqual(ruleBody.ProductId, get.Response.ProductId);
            Assert.AreEqual(ruleBody.Quantity, get.Response.Quantity);
        }
    }
}
