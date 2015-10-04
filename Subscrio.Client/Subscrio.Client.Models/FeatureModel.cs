using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscrio.Client.Models
{
    public class FeatureModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public int? FeatureCategoryId { get; set; }
        public string FeatureCategoryName { get; set; }
        public int DataType { get; set; }
        public int Priority { get; set; }
        public bool IsDeleted { get; set; }
        public string Value { get; set; }
    }
}
