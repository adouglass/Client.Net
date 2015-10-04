using System.Collections.Generic;
using System.Dynamic;

namespace Subscrio.Client
{
    public class DynamicDictionary : DynamicObject
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

    }
}