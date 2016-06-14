using System;
using System.Collections.Generic;
using System.Dynamic;

namespace SubscriptionApp.Client
{
    public class DynamicDictionary : DynamicObject, ISubscriber
    {
        private readonly Dictionary<string, object> dictionary;

        public DynamicDictionary(Dictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            return dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            dictionary[binder.Name] = value;

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dictionary.Keys;
        }

        public object GetMemberValue(string propertyName)
        {
            return this.dictionary[propertyName];
        }

        public long SubscriptionTypeId
        {
            get { return (dictionary["SubscriptionTypeId"] as long?).GetValueOrDefault(); }
            set { dictionary["SubscriptionTypeId"] = value; }
        }

        public long Id
        {
            get { return (dictionary["Id"] as long?).GetValueOrDefault(); }
            set { dictionary["Id"] = value; }
        }
        public string Name
        {
            get { return dictionary["Name"] as string; }
            set { dictionary["Name"] = value; }
        }
        public string ApplicationId
        {
            get { return dictionary["ApplicationId"] as string; }
            set { dictionary["ApplicationId"] = value; }
        }
        public string Key
        {
            get { return dictionary["Key"] as string; }
            set { dictionary["Key"] = value; }
        }
        public DateTime ExpirationDate
        {
            get { return (dictionary["ExpirationDate"] as DateTime?).GetValueOrDefault(); }
            set { dictionary["ExpirationDate"] = value; }
        }
        public string BillingSystemIdentifier
        {
            get { return dictionary["BillingSystemIdentifier"] as string; }
            set { dictionary["BillingSystemIdentifier"] = value; }
        }
        public int BillingSystemType
        {
            get { return (dictionary["BillingSystemType"] as int?).GetValueOrDefault(); }
            set { dictionary["BillingSystemType"] = value; }
        }
        public bool IsExpired
        {
            get { return (dictionary["IsExpired"] as bool?).GetValueOrDefault(); }
        }
        public bool IsAlive
        {
            get { return (dictionary["IsAlive"] as bool?).GetValueOrDefault(); }
        }
    }
}