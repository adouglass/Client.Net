using System;
using System.Collections.Generic;

namespace SubscriptionApp.Client.Models
{
    public class SubscriberModel
    {
        public SubscriberModel()
        {
            Features = new List<FeatureModel>();
        }
        public int Id { get; set; }
        public int SubscriptionTypeId { get; set; }
        public int BillingSystemType { get; set; }
        public string BillingSystemIdentifier { get; set; }
        public int CompanyId { get; set; }
        public string Key { get; set; }
        public string ApplicationId { get; set; }
        public string Name { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int DefaultRevertTo { get; set; }
        public bool DefaultRevertOnExpiration { get; set; }
        public bool DefaultNeverExpire { get; set; }
        public int DefaultGracePeriod { get; set; }
        public bool DefaultResetFeaturesOnRenewal { get; set; }
        public List<FeatureModel> Features { get; set; }
    }

    
}
