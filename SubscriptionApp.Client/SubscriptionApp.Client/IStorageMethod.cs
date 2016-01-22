using System.Collections.Generic;
using SubscriptionApp.Client.Models;

namespace SubscriptionApp.Client
{
    public interface IStorageMethod
    {
        SubscriberModel GetByKey(string key);
        SubscriberModel GetByApplicationId(string appId);
        void AddOrUpdateSubscriber(SubscriberModel model);
        void SubscriberRemoved(SubscriberModel model);
        void UpdateConfiguration(Configuration config);
        void AddOrUpdateSubscribers(List<SubscriberModel> subscribers);
        Configuration GetConfiguration();
        List<SubscriberModel> GetAllSubscriptions();
    }
}
