using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Location
    {
        public Location()
        {
            Posts = new HashSet<Post>();
        }

        public int LocationId { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
