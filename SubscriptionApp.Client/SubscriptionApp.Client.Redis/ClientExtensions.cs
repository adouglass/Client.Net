using StackExchange.Redis;

namespace SubscriptionApp.Client.Redis
{
    public static class ClientExtensions
    {
        public static SubscriptionClient UseRedis(this SubscriptionClient client,string redisConnectionString)
        {
            client.StorageMethod = new RedisStorage(redisConnectionString);
            return client;
        }
        public static SubscriptionClient UseRedis(this SubscriptionClient client,ConfigurationOptions options)
        {
            client.StorageMethod = new RedisStorage(options);
            return client;
        }
    }
}
