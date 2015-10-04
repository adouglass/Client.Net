using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;
using Subscrio.Client.Models;

namespace Subscrio.Client.Redis
{
    public class RedisStorage : IStorageMethod
    {
        private readonly string _connectionString;
        private readonly ConfigurationOptions _connectionOptions;
        private const string SUBSCRIO_CONFIG_KEY = "subscrio_configuration";
        private const string SUBSCRIO_ALL_KEY = "subscrio_all_subscribers";
        private const string HASH_EXPIRATION = "subscrio_hash_expiration";
        private IDatabase _database
        {
            get
            {
                var multiplexer = string.IsNullOrEmpty(_connectionString) ? ConnectionHandler.RetrieveMultiplexer(_connectionOptions) : ConnectionHandler.RetrieveMultiplexer(_connectionString);
                return multiplexer.GetDatabase();
            }
        }

        public RedisStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RedisStorage(ConfigurationOptions options)
        {
            _connectionOptions = options;
        }
        public SubscriberModel GetByKey(string key)
        {
            var redisValue = _database.StringGet(GetRedisKeyForKey(key));
            return !redisValue.HasValue ? null : JsonConvert.DeserializeObject<SubscriberModel>(redisValue);
        }

        public SubscriberModel GetByApplicationId(string appId)
        {
            var redisValue = _database.StringGet(GetRedisKeyForAppId(appId));
            return !redisValue.HasValue ? null : JsonConvert.DeserializeObject<SubscriberModel>(redisValue);
        }

        public void AddOrUpdateSubscriber(SubscriberModel model)
        {
            var config = GetConfiguration();
            var expirationTicks = config?.Company?.CacheTimeoutTicks;
            var expiration = expirationTicks != null ? new TimeSpan?(new TimeSpan(expirationTicks.Value)) : null;
            var stringModel = JsonConvert.SerializeObject(model);

            _database.StringSet(GetRedisKeyForAppId(model.ApplicationId), stringModel, expiration);
            _database.StringSet(GetRedisKeyForKey(model.Key), stringModel, expiration);
            _database.HashSet(SUBSCRIO_ALL_KEY, GetRedisKeyForKey(model.Key), stringModel);
        }

        public void UpdateConfiguration(Configuration config)
        {
            var stringModel = JsonConvert.SerializeObject(config);
            _database.StringSetAsync(SUBSCRIO_CONFIG_KEY, stringModel);
        }

        public void AddOrUpdateSubscribers(List<SubscriberModel> subscribers)
        {
            var config = GetConfiguration();
            var expirationTicks = config?.Company?.CacheTimeoutTicks;
            foreach (var subscriberModel in subscribers)
            {
                AddOrUpdateSubscriber(subscriberModel);
            }
            _database.StringSet(HASH_EXPIRATION, expirationTicks == null ? 0 : DateTime.UtcNow.Ticks + expirationTicks);

        }

        public Configuration GetConfiguration()
        {
            var redisValue = _database.StringGet(SUBSCRIO_CONFIG_KEY);
            return !redisValue.HasValue ? null : JsonConvert.DeserializeObject<Configuration>(redisValue);
        }

        public List<SubscriberModel> GetAllSubscriptions()
        {
            var hashExpiration = (long?)_database.StringGet(HASH_EXPIRATION);
            if (hashExpiration == null) return null;
            if (hashExpiration != 0 && DateTime.UtcNow.Ticks > hashExpiration)
            {
                var all = _database.HashGetAll(SUBSCRIO_ALL_KEY);
                foreach (var hashEntry in all)
                {
                    _database.HashDelete(SUBSCRIO_ALL_KEY, hashEntry.Name);
                }
                return null;
            }
            var values = _database.HashValues(SUBSCRIO_ALL_KEY);
            return values.Select(x => JsonConvert.DeserializeObject<SubscriberModel>(x)).ToList();
        }

        private string GetRedisKeyForKey(string key)
        {
            return $"SubscrioKey:{key}";
        }

        private string GetRedisKeyForAppId(string appId)
        {
            return $"SubscrioAppId:{appId}";
        }

    }
}
