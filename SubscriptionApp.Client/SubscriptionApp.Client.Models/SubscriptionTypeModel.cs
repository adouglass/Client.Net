using System.Collections.Generic;

namespace SubscriptionApp.Client.Models
{
    public class SubscriptionTypeModel
    {
        public SubscriptionTypeModel()
        {
            Features = new List<FeatureModel>();
            BillingSystemLinks = new List<BillingSystemLink>();
        }
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; } 
        public bool IsDeleted { get; set; }
        public int Priority { get; set; }
        public int BillingSystemType { get; set; }
        public long? TimeToExpireTicks { get; set; }
        public int DefaultRevertTo { get; set; }
        public bool DefaultRevertOnExpiration { get; set; }
        public bool DefaultNeverExpire { get; set; }
        public int DefaultGracePeriod { get; set; }
        public bool DefaultResetFeaturesOnRenewal { get; set; }
        public List<FeatureModel> Features { get; set; }
        public List<BillingSystemLink> BillingSystemLinks { get; set; }
    }

    public class BillingSystemLink
    {
        public int BillingSystemType { get; set; }
        public string Link { get; set; }
    }
}
