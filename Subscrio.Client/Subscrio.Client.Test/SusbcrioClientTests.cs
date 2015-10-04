using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Subscrio.Client.Models;
using Subscrio.Client.Redis;
using Subscrio.Client.Services;

namespace Subscrio.Client.Test
{
    [TestFixture]
    public class SusbcrioClientTests
    {
        private Mock<WebClientService> _mockService;
        private SubscrioClient _client;

        private static readonly string SUSBCRIBER_JSON =
            $"{{\"Id\":3,\"SubscriptionTypeId\":1,\"BillingSystemType\":0,\"CompanyId\":1,\"Key\":\"Tuttle\",\"Name\":\"Tuttle\",\"ApplicationId\":\"Tuttle\",\"ExpirationDate\":\"2016-08-04T00:00:00Z\",\"DefaultRevertTo\":0,\"DefaultRevertOnExpiration\":false,\"DefaultNeverExpire\":false,\"DefaultGracePeriod\":0,\"DefaultResetFeaturesOnRenewal\":false,\"Features\":[{{\"Id\":2,\"Name\":\"Routing Enabled\",\"PropertyName\":\"RoutingEnabled\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":0,\"IsDeleted\":false,\"Value\":\"True\",\"BillingSystemLinks\":[]}},{{\"Id\":3,\"Name\":\"New Version Date\",\"PropertyName\":\"NewVersionDate\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":2,\"IsDeleted\":false,\"Value\":\"09/04/2015\",\"BillingSystemLinks\":[]}},{{\"Id\":4,\"Name\":\"Plan Name\",\"PropertyName\":\"PlanName\",\"CompanyId\":0,\"FeatureCategoryId\":null,\"CategoryName\":\"\",\"Priority\":0,\"DataType\":3,\"IsDeleted\":false,\"Value\":\"the name\",\"BillingSystemLinks\":[]}},{{\"Id\":5,\"Name\":\"Daily Rate\",\"PropertyName\":\"DailyRate\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":4,\"IsDeleted\":false,\"Value\":\"44.44\",\"BillingSystemLinks\":[]}},{{\"Id\":1,\"Name\":\"Enterprise Chat Limit\",\"PropertyName\":\"EnterpriseChatLimit\",\"CompanyId\":0,\"FeatureCategoryId\":1,\"CategoryName\":\"freemium\",\"Priority\":0,\"DataType\":1,\"IsDeleted\":false,\"Value\":\"222\",\"BillingSystemLinks\":[]}}]}}";

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
            _client = new SubscrioClient(_mockService.Object).UseRedis("localhost");
        }

        [Test]
        public void ShouldCreatePropertiesBasedUponFeatures()
        {
            _mockService.Setup(x => x.GetSubscriptionByKey(It.IsAny<string>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscrioClient(_mockService.Object).UseRedis("localhost");
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
            result = _client.GetSubscriptionByKey("shdflsf");
        }

        [Test]
        public void ShouldThrowExceptionWhenNotConfigured()
        {
            try
            {
                _client = new SubscrioClient(_mockService.Object);
                var result = _client.GetConfiguration();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Subscrio client must be configured with a storage method", ex.Message);
            }
        }

        [Test]
        public void ShouldCreateSubscriber()
        {
            _mockService.Setup(x => x.CreateSubscription(It.IsAny<SubscriberModel>())).Returns(SUSBCRIBER_JSON);
            _client = new SubscrioClient(_mockService.Object).UseRedis("localhost");
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
            _client = new SubscrioClient(_mockService.Object).UseRedis("localhost");
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
            _client = new SubscrioClient(_mockService.Object).UseRedis("localhost");
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
            var badClient = new SubscrioClient(_mockService.Object).UseRedis("localhost");
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
            _client = new SubscrioClient(_mockService.Object).UseRedis("localhost");
            try
            {
                _client.UpdateSubscription((new TestFeatures(new Dictionary<string, object> { { "Key", "Tuttle" } })));
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Cannot update subscription, original subscribption not found", ex.Message);
            }
        }

    }

    public class TestFeatures: DynamicDictionary
    {
        public TestFeatures(Dictionary<string, object> dictionary) : base(dictionary)
        {
        }
    }

}
