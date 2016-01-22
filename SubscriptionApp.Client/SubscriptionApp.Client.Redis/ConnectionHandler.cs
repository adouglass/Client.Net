using StackExchange.Redis;

namespace SubscriptionApp.Client.Redis
{
    public static class ConnectionHandler
    {
        private static ConnectionMultiplexer _multiplexer { get; set; }

        public static ConnectionMultiplexer RetrieveMultiplexer(string connectionString)
        {
           if(_multiplexer != null) return _multiplexer;

            _multiplexer = ConnectionMultiplexer.Connect(connectionString);
            return _multiplexer;
        }

        public static ConnectionMultiplexer RetrieveMultiplexer(ConfigurationOptions config)
        {
           if(_multiplexer != null) return _multiplexer;

            _multiplexer = ConnectionMultiplexer.Connect(config);
            return _multiplexer;
        }
    }
}
