using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class WeightType
    {
        public WeightType()
        {
            Posts = new HashSet<Post>();
        }

        public int WeightId { get; set; }
        public string WeightName { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
