using System;

namespace SubscriptionApp.Client
{
    public interface ISubscriber
    {
        long SubscriptionTypeId { get; set; }
        long Id { get; set; }
        string Name { get; set; }
        string ApplicationId { get; set; }
        string Key { get; set; }
        DateTime ExpirationDate { get; set; }
        string BillingSystemIdentifier { get; set; }
        int BillingSystemType { get; set; }
        bool IsExpired { get; }
    }
}