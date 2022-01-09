using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            Posts = new HashSet<Post>();
        }

        public int CategoryId { get; set; }
        public string CateName { get; set; }
        public bool? IsDelete { get; set; }
        public string Image { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
