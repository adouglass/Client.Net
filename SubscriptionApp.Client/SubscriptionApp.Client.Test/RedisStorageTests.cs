using System.Collections.Generic;
using NUnit.Framework;
using StackExchange.Redis;
using SubscriptionApp.Client.Models;
using SubscriptionApp.Client.Redis;

namespace SubscriptionApp.Client.Test
{
    [TestFixture]
    class RedisStorageTests
    {
        private RedisStorage _redis;

        [SetUp]
        public void SetUp()
        {
            _redis = new RedisStorage("localhost");
            _redis.UpdateConfiguration(new Configuration
            {
                Company = new CompanyModel
                {
                    CacheTimeoutTicks = 50000000
                }
            });
            TestHelper.ClearRedis();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            TestHelper.RemoveConfiguration();
        }

        [Test]
        public void ShouldStoreAndRetrieveSubscriber()
        {
            var model = new SubscriberModel
            {
                ApplicationId = "AppTuttle",
                Key = "KeyTuttle",
            };
            _redis.AddOrUpdateSubscriber(model);
            var result = _redis.GetByApplicationId("AppTuttle");
            Assert.IsNotNull(result);
            result = _redis.GetByKey("KeyTuttle");
            Assert.IsNotNull(result);
        }

        [Test]
        public void ShouldHaveExpiredAndReturnNull()
        {
            var model = new SubscriberModel
            {
                ApplicationId = "AppTuttle",
                Key = "KeyTuttle",
            };
            _redis.AddOrUpdateSubscriber(model);
            System.Threading.Thread.Sleep(6000);
            var result = _redis.GetByApplicationId("AppTuttle");
            Assert.IsNull(result);
            result = _redis.GetByKey("KeyTuttle");
            Assert.IsNull(result);
        }

        [Test]
        public void ShouldAddOrUpdateMultipleSubscribers()
        {
            var subscribers = new List<SubscriberModel>
            {
                new SubscriberModel
                {
                    ApplicationId = "1",
                    Key = "1",
                },
                new SubscriberModel
                {
                    ApplicationId = "2",
                    Key = "2",
                },
            };
            _redis.AddOrUpdateSubscribers(subscribers);
            var result = _redis.GetByApplicationId("1");
            Assert.IsNotNull(result);
            result = _redis.GetByKey("1");
            Assert.IsNotNull(result);
            result = _redis.GetByApplicationId("2");
            Assert.IsNotNull(result);
            result = _redis.GetByKey("2");
            Assert.IsNotNull(result);
            subscribers[1].CompanyId = 44;
            _redis.AddOrUpdateSubscribers(subscribers);
            result = _redis.GetByApplicationId("2");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CompanyId, 44);
            result = _redis.GetByKey("2");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CompanyId, 44);
        }

        [Test]
        public void ShouldGetAll()
        {
            var subscribers = new List<SubscriberModel>
            {
                new SubscriberModel
                {
                    ApplicationId = "1",
                    Key = "1",
                },
                new SubscriberModel
                {
                    ApplicationId = "2",
                    Key = "2",
                },
            };
            _redis.AddOrUpdateSubscribers(subscribers);
            var result = _redis.GetAllSubscriptions();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ShouldReturnNullOnHashExpiration()
        {
            var subscribers = new List<SubscriberModel>
            {
                new SubscriberModel
                {
                    ApplicationId = "1",
                    Key = "1",
                },
                new SubscriberModel
                {
                    ApplicationId = "2",
                    Key = "2",
                },
            };
            _redis.AddOrUpdateSubscribers(subscribers);
            System.Threading.Thread.Sleep(6000);
            var result = _redis.GetAllSubscriptions();
            Assert.IsNull(result);
        }
    }

    public static class TestHelper
    {
        public static void ClearRedis()
        {
            var db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
            var all = db.HashGetAll("subscrio_all_subscribers");
            foreach (var hashEntry in all)
            {
                db.HashDelete("subscrio_all_subscribers", hashEntry.Name);
            }
        }
        public static void RemoveConfiguration()
        {
            var db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
            db.KeyDelete("subscrio_configuration");
        }
    }
}
