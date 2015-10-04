using StackExchange.Redis;

namespace Subscrio.Client.Redis
{
    public static class ClientExtensions
    {
        public static SubscrioClient UseRedis(this SubscrioClient client,string redisConnectionString)
        {
            client.StorageMethod = new RedisStorage(redisConnectionString);
            return client;
        }
        public static SubscrioClient UseRedis(this SubscrioClient client,ConfigurationOptions options)
        {
            client.StorageMethod = new RedisStorage(options);
            return client;
        }
    }
}
