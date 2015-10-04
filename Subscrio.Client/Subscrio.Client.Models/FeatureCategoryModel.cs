using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscrio.Client.Models
{
    public class FeatureCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool IsDeleted { get; set; }
    }
}
