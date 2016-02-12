using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SubscriptionApp.Client.Models;
using SubscriptionApp.Client.Redis;
using SubscriptionApp.Client.Services;

namespace SubscriptionApp.Client.Test
{
    [TestFixture]
    public class SusbcrioClientTests
    {
        private Mock<WebClientService> _mockService;
        private SubscriptionClient _client;

        private static readonly string SUSBCRIBER_JSON =
            $"{{\"Id\":3,\"SubscriptionTypeId\":1,\"BillingSystemType\":0,\"CompanyId\":1,\"Key\":\"Tuttle\",\"Name\":\"Tuttle\",\"ApplicationId\":\"Tuttle\",\"ExpirationDate\":\"2016-08-04T00:00:00Z\",\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false,\"Features\":[{{\"Id\":2,\"Name\":\"Routing Enabled\",\"PropertyName\":\"RoutingEnabled\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":0,\"IsDeleted\":false,\"Value\":\"True\",\"BillingSystemLinks\":[]}},{{\"Id\":3,\"Name\":\"New Version Date\",\"PropertyName\":\"NewVersionDate\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":2,\"IsDeleted\":false,\"Value\":\"09/04/2015\",\"BillingSystemLinks\":[]}},{{\"Id\":4,\"Name\":\"Plan Name\",\"PropertyName\":\"PlanName\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":3,\"IsDeleted\":false,\"Value\":\"the name\",\"BillingSystemLinks\":[]}},{{\"Id\":5,\"Name\":\"Daily Rate\",\"PropertyName\":\"DailyRate\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":4,\"IsDeleted\":false,\"Value\":\"44.44\",\"BillingSystemLinks\":[]}},{{\"Id\":1,\"Name\":\"Enterprise Chat Limit\",\"PropertyName\":\"EnterpriseChatLimit\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":1,\"IsDeleted\":false,\"Value\":\"222\",\"BillingSystemLinks\":[]}}]}}";

        private static readonly string SUSBCRIBER_JSON_NULL =
            $"{{\"Id\":3,\"SubscriptionTypeId\":1,\"BillingSystemType\":0,\"CompanyId\":1,\"Key\":\"Tuttle\",\"Name\":\"TuttleNull\",\"ApplicationId\":\"TuttleNull\",\"ExpirationDate\":\"2016-08-04T00:00:00Z\",\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false,\"Features\":[{{\"Id\":2,\"Name\":\"Routing Enabled\",\"PropertyName\":\"RoutingEnabled\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":0,\"IsDeleted\":false,\"Value\":null,\"BillingSystemLinks\":[]}},{{\"Id\":3,\"Name\":\"New Version Date\",\"PropertyName\":\"NewVersionDate\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":2,\"IsDeleted\":false,\"Value\":null,\"BillingSystemLinks\":[]}},{{\"Id\":4,\"Name\":\"Plan Name\",\"PropertyName\":\"PlanName\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":3,\"IsDeleted\":false,\"Value\":null,\"BillingSystemLinks\":[]}},{{\"Id\":5,\"Name\":\"Daily Rate\",\"PropertyName\":\"DailyRate\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":4,\"IsDeleted\":false,\"Value\":null,\"BillingSystemLinks\":[]}},{{\"Id\":1,\"Name\":\"Enterprise Chat Limit\",\"PropertyName\":\"EnterpriseChatLimit\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":1,\"IsDeleted\":false,\"Value\":null,\"BillingSystemLinks\":[]}}]}}";

        private static readonly string SUSBCRIBERS_JSON =
            $"[{{\"Id\":3,\"SubscriptionTypeId\":1,\"BillingSystemType\":0,\"CompanyId\":1,\"Key\":\"Tuttle\",\"Name\":\"Tuttle\",\"ApplicationId\":\"Tuttle\",\"ExpirationDate\":\"2016-08-04T00:00:00Z\",\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false,\"Features\":[{{\"Id\":2,\"Name\":\"Routing Enabled\",\"PropertyName\":\"RoutingEnabled\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":0,\"IsDeleted\":false,\"Value\":\"True\",\"BillingSystemLinks\":[]}},{{\"Id\":3,\"Name\":\"New Version Date\",\"PropertyName\":\"NewVersionDate\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":2,\"IsDeleted\":false,\"Value\":\"09/04/2015\",\"BillingSystemLinks\":[]}},{{\"Id\":4,\"Name\":\"Plan Name\",\"PropertyName\":\"PlanName\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":3,\"IsDeleted\":false,\"Value\":\"the name\",\"BillingSystemLinks\":[]}},{{\"Id\":5,\"Name\":\"Daily Rate\",\"PropertyName\":\"DailyRate\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":4,\"IsDeleted\":false,\"Value\":\"44.44\",\"BillingSystemLinks\":[]}},{{\"Id\":1,\"Name\":\"Enterprise Chat Limit\",\"PropertyName\":\"EnterpriseChatLimit\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":1,\"IsDeleted\":false,\"Value\":\"222\",\"BillingSystemLinks\":[]}}]}}, {{\"Id\":4,\"SubscriptionTypeId\":1,\"BillingSystemType\":0,\"CompanyId\":1,\"Key\":\"Another\",\"Name\":\"Another\",\"ApplicationId\":\"Another\",\"ExpirationDate\":\"2016-08-05T03:59:59.441Z\",\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false,\"Features\":[{{\"Id\":1,\"Name\":\"Enterprise Chat Limit\",\"PropertyName\":\"EnterpriseChatLimit\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"FeatureCategoryName\":\"freemium\",\"DataType\":1,\"Priority\":0,\"IsDeleted\":false,\"Value\":\"22\"}},{{\"Id\":2,\"Name\":\"Routing Enabled\",\"PropertyName\":\"RoutingEnabled\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"FeatureCategoryName\":\"\",\"DataType\":0,\"Priority\":0,\"IsDeleted\":false,\"Value\":\"True\"}},{{\"Id\":3,\"Name\":\"New Version Date\",\"PropertyName\":\"NewVersionDate\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"FeatureCategoryName\":\"\",\"DataType\":2,\"Priority\":0,\"IsDeleted\":false,\"Value\":\"09/05/2015\"}},{{\"Id\":4,\"Name\":\"Plan Name\",\"PropertyName\":\"PlanName\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"FeatureCategoryName\":\"\",\"DataType\":3,\"Priority\":0,\"IsDeleted\":false,\"Value\":\"basic\"}},{{\"Id\":5,\"Name\":\"Daily Rate\",\"PropertyName\":\"DailyRate\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"FeatureCategoryName\":\"freemium\",\"DataType\":4,\"Priority\":0,\"IsDeleted\":false,\"Value\":\"55.55\"}}]}}]";

        private static readonly string CONFIGURATION_JSON =
            $"{{\"Company\":{{\"Id\":1,\"Name\":\"velaro\",\"Token\":\"wcd1ikGGf0yNqxvl7R9RBg\",\"WebhookUrl\":\"\",\"CacheTimeoutTicks\":864000000000}},\"SubscriptionTypes\":[{{\"Id\":1,\"Key\":null,\"Name\":\"Basic\",\"IsDeleted\":true,\"Priority\":0,\"BillingSystemType\":0,\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false}},{{\"Id\":2,\"Key\":null,\"Name\":\"sdafasl\",\"IsDeleted\":true,\"Priority\":0,\"BillingSystemType\":0,\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false}},{{\"Id\":3,\"Key\":null,\"Name\":\"ldfhdsald;\",\"IsDeleted\":true,\"Priority\":0,\"BillingSystemType\":0,\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false}},{{\"Id\":4,\"Key\":null,\"Name\":\"sdlfkhsadl;\",\"IsDeleted\":true,\"Priority\":0,\"BillingSystemType\":0,\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false}}],\"FeatureCategories\":[{{\"Id\":1,\"Name\":\"freemium\",\"Priority\":0,\"IsDeleted\":false}}],\"Features\":[{{\"Id\":1,\"Name\":\"Enterprise Chat Limit\",\"PropertyName\":\"EnterpriseChatLimit\",\"FeatureCategoryId\":1,\"FeatureCategoryName\":\"freemium\",\"DataType\":1,\"Priority\":0,\"IsDeleted\":false,\"Value\":null}},{{\"Id\":2,\"Name\":\"Routing Enabled\",\"PropertyName\":\"RoutingEnabled\",\"FeatureCategoryId\":null,\"FeatureCategoryName\":null,\"DataType\":0,\"Priority\":1,\"IsDeleted\":false,\"Value\":null}},{{\"Id\":3,\"Name\":\"New Version Date\",\"PropertyName\":\"NewVersionDate\",\"FeatureCategoryId\":null,\"FeatureCategoryName\":null,\"DataType\":2,\"Priority\":2,\"IsDeleted\":false,\"Value\":null}},{{\"Id\":4,\"Name\":\"Plan Name\",\"PropertyName\":\"PlanName\",\"FeatureCategoryId\":null,\"FeatureCategoryName\":null,\"DataType\":3,\"Priority\":3,\"IsDeleted\":false,\"Value\":null}},{{\"Id\":5,\"Name\":\"Daily Rate\",\"PropertyName\":\"DailyRate\",\"FeatureCategoryId\":1,\"FeatureCategoryName\":\"freemium\",\"DataType\":4,\"Priority\":4,\"IsDeleted\":false,\"Value\":null}}]}}";

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<WebClientService>();
            _mockService.Setup(x => x.GetSubscriptions()).Returns(string.Empty);
            _mockService.Setup(x => x.GetSubscriptionByKey(It.IsAny<string>())).Returns(string.Empty);
            _mockService.Setup(x => x.GetSubscriptionByApplicationId(It.IsAny<string>())).Returns(string.Empty);
            _mockService.Setup(x => x.GetConfiguration()).Returns(CONFIGURATION_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
        }

        [Test]
        public void ShouldCreatePropertiesBasedUponFeatures()
        {
            _mockService.Setup(x => x.GetSubscriptionByKey(It.IsAny<string>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            var result = _client.GetSubscriptionByKey("Tuttle");
            Assert.AreEqual(true, result.RoutingEnabled);
            Assert.AreEqual(44.44m, result.DailyRate);
            Assert.AreEqual(222, result.EnterpriseChatLimit);
            Assert.AreEqual("the name", result.PlanName);
            Assert.AreEqual(2015, result.NewVersionDate.Year);
            Assert.AreEqual(9, result.NewVersionDate.Month);
            Assert.AreEqual(4, result.NewVersionDate.Day);
            Assert.AreEqual(1, result.SubscriptionTypeId);
            Assert.AreEqual("Tuttle", result.Name);
            Assert.AreEqual(false, result.IsExpired);
        }

        [Test]
        public void ShouldCreatePropertiesBasedUponFeaturesForStronglyTypedModel()
        {
            _mockService.Setup(x => x.GetSubscriptionByKey(It.IsAny<string>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            var result = _client.GetSubscriptionByKey<TestClass>("Tuttle");
            Assert.AreEqual(true, result.RoutingEnabled);
            Assert.AreEqual(44.44m, result.DailyRate);
            Assert.AreEqual(222, result.EnterpriseChatLimit);
            Assert.AreEqual("the name", result.PlanName);
            Assert.AreEqual(2015, result.NewVersionDate.Year);
            Assert.AreEqual(9, result.NewVersionDate.Month);
            Assert.AreEqual(4, result.NewVersionDate.Day);
            Assert.AreEqual(1, result.SubscriptionTypeId);
            Assert.AreEqual("Tuttle", result.Name);
            Assert.AreEqual(false, result.IsExpired);
        }

        [Test]
        public void ShouldUpdateLocalModelForStronglyTypedModelOrDynamic()
        {
            _mockService.Setup(x => x.GetSubscriptionByKey(It.IsAny<string>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            var result = _client.GetSubscriptionByKey<TestClass>("Tuttle");
            result.PlanName = "Steeeeeeeve";
            _client.UpdateLocalSubscription(result);
            result = _client.GetSubscriptionByKey<TestClass>("Tuttle");
            Assert.AreEqual("Steeeeeeeve", result.PlanName);

            var dynamicResult = _client.GetSubscriptionByKey("Tuttle");
            dynamicResult.PlanName = "Tuttle";
            _client.UpdateLocalSubscription(dynamicResult);
            result = _client.GetSubscriptionByKey<TestClass>("Tuttle");
            Assert.AreEqual("Tuttle", result.PlanName);
        }

        [Test]
        public void ShouldReturnNullForFeaturesThatAreNull()
        {
            _mockService.Setup(x => x.GetSubscriptionByKey(It.IsAny<string>())).Returns(SUSBCRIBER_JSON_NULL);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            var result = _client.GetSubscriptionByKey("TuttleNull");
            Assert.IsNull(result.RoutingEnabled);
            Assert.IsNull(result.DailyRate);
            Assert.IsNull(result.EnterpriseChatLimit);
            Assert.IsNull(result.PlanName);
            Assert.IsNull(result.NewVersionDate);
            Assert.AreEqual(1, result.SubscriptionTypeId);
            Assert.AreEqual("TuttleNull", result.Name);
            Assert.AreEqual(false, result.IsExpired);
        }

        [Test]
        public void ShouldCreateSubscriber()
        {
            _mockService.Setup(x => x.CreateSubscription(It.IsAny<SubscriberModel>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            var result = _client.CreateSubscription(1,"uniqueAppId","name");
            Assert.AreEqual(true, result.RoutingEnabled);
            Assert.AreEqual(44.44m, result.DailyRate);
            Assert.AreEqual(222, result.EnterpriseChatLimit);
            Assert.AreEqual("the name", result.PlanName);
            Assert.AreEqual(2015, result.NewVersionDate.Year);
            Assert.AreEqual(9, result.NewVersionDate.Month);
            Assert.AreEqual(4, result.NewVersionDate.Day);
        }

        [Test]
        public void ShouldUpdateSubscriber()
        {
            _mockService.Setup(x => x.GetSubscriptions()).Returns(SUSBCRIBERS_JSON);
            _mockService.Setup(x => x.UpdateSubscription(It.IsAny<SubscriberModel>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            var result = _client.UpdateSubscription(new TestFeatures (new Dictionary<string, object> { {"Key", "Tuttle"} }));
            Assert.AreEqual(true, result.RoutingEnabled);
            Assert.AreEqual(44.44m, result.DailyRate);
            Assert.AreEqual(222, result.EnterpriseChatLimit);
            Assert.AreEqual("the name", result.PlanName);
            Assert.AreEqual(2015, result.NewVersionDate.Year);
            Assert.AreEqual(9, result.NewVersionDate.Month);
            Assert.AreEqual(4, result.NewVersionDate.Day);
        }

        [Test]
        public void ShouldThrowGeneralExceptionOnCreateSubscriber()
        {
            _mockService.Setup(x => x.GetConfiguration()).Returns(string.Empty);
            _mockService.Setup(x => x.CreateSubscription(It.IsAny<SubscriberModel>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            try
            {
                _client.CreateSubscription(1,"uniqueAppId","name");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Missing configuration or subscription types, no subscription types found", ex.Message);
            }
        }

        [Test]
        public void ShouldThrowSpecificExceptionOnCreateSubscriber()
        {
            _mockService.Setup(x => x.CreateSubscription(It.IsAny<SubscriberModel>())).Returns(SUSBCRIBER_JSON);
            var badClient = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            try
            {
                badClient.CreateSubscription(100, "uniqueAppId", "name");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Subscription type with id of: 100 was not found", ex.Message);
            }
        }

        [Test]
        public void ShouldThrowGeneralExceptionOnUpdateSubscriber()
        {
            _mockService.Setup(x => x.GetSubscriptions()).Returns(SUSBCRIBERS_JSON);
            _mockService.Setup(x => x.UpdateSubscription(It.IsAny<SubscriberModel>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            try
            {
                _client.UpdateSubscription((new TestFeatures(new Dictionary<string, object> { { "Key", "asdlfhasdklfahsdl" } })));
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Cannot update subscription, original subscription not found", ex.Message);
            }
        }

        [Test]
        public void ShouldRemoveSubscribersOnUpdateAll()
        {
            _mockService.Setup(x => x.GetSubscriptions()).Returns(SUSBCRIBERS_JSON);
            _client = new SubscriptionClient(_mockService.Object).UseRedis("localhost");
            _client.SubscriptionUpdated(new SubscriberModel {ApplicationId = "1234", Key = "5678"});
            _client.WebhookUpdate(new SubscriberWebhookModel {Action = "updateall"});
            System.Threading.Thread.Sleep(1000);
            var result = _client.GetSubscriptionByKey("5678");
            Assert.IsNull(result);
        }
    }

    public class TestFeatures: DynamicDictionary
    {
        public TestFeatures(Dictionary<string, object> dictionary) : base(dictionary)
        {

            dictionary.Add("SubscriptionTypeId", null);
            dictionary.Add("Id", null);
            dictionary.Add("Name", null);
            dictionary.Add("ApplicationId", null);
            dictionary.Add("ExpirationDate", null);
            dictionary.Add("BillingSystemIdentifier", null);
            dictionary.Add("BillingSystemType", null);
            dictionary.Add("IsExpired",null);
        }
    }

    public class TestClass : ISubscriber
    {
        public long SubscriptionTypeId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string ApplicationId { get; set; }
        public string Key { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string BillingSystemIdentifier { get; set; }
        public int BillingSystemType { get; set; }
        public bool IsExpired { get; }

        public bool RoutingEnabled { get; set; }
        public decimal DailyRate { get; set; }
        public int EnterpriseChatLimit { get; set; }
        public string PlanName { get; set; }
        public DateTime NewVersionDate { get; set; }

    }

}
