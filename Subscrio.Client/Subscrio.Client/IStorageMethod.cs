using System.Collections.Generic;
using Subscrio.Client.Models;

namespace Subscrio.Client
{
    public interface IStorageMethod
    {
        SubscriberModel GetByKey(string key);
        SubscriberModel GetByApplicationId(string appId);
        void AddOrUpdateSubscriber(SubscriberModel model);
        void UpdateConfiguration(Configuration config);
        void AddOrUpdateSubscribers(List<SubscriberModel> subscribers);
        Configuration GetConfiguration();
        List<SubscriberModel> GetAllSubscriptions();
    }
}
