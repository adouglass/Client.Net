using System.Collections.Generic;
using SubscriptionApp.Client.Models;

namespace SubscriptionApp.Client
{
    public class FakeStorageMethod: IStorageMethod
    {
        public SubscriberModel GetByKey(string key)
        {
            return null;
        }

        public SubscriberModel GetByApplicationId(string appId)
        {
            return null;
        }

        public void AddOrUpdateSubscriber(SubscriberModel model)
        {
        }

        public void SubscriberRemoved(SubscriberModel model)
        {
        }

        public void UpdateConfiguration(Configuration config)
        {
        }

        public void AddOrUpdateSubscribers(List<SubscriberModel> subscribers)
        {
        }

        public Configuration GetConfiguration()
        {
            return null;
        }

        public List<SubscriberModel> GetAllSubscriptions()
        {
            return null;
        }
    }
}
