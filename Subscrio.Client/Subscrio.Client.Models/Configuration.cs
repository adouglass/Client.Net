using System.Collections.Generic;

namespace Subscrio.Client.Models
{
    public class Configuration
    {
        public CompanyModel Company { get; set; }
        public List<SubscriptionTypeModel> SubscriptionTypes { get; set; } 
        public List<FeatureCategoryModel> FeatureCategories { get; set; } 
        public List<FeatureModel> Features { get; set; }
    }
}
