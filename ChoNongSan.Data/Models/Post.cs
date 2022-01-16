using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Post
    {
        public Post()
        {
            ImagePosts = new HashSet<ImagePost>();
            Loves = new HashSet<Love>();
            Reviews = new HashSet<Review>();
        }

        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int WeightId { get; set; }
        public int? WeightNumber { get; set; }
        public decimal? Price { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int AccountId { get; set; }
        public int? StatusPost { get; set; }
        public DateTime? PostTime { get; set; }
        public bool? IsDeliver { get; set; }
        public string Quality { get; set; }
        public int? ViewCount { get; set; }
        public string Reason { get; set; }
        public bool? IsHidden { get; set; }
        public int? Ctv { get; set; }
        public int CategoryId { get; set; }
        public int LocationId { get; set; }
        public int? Expiry { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
        public virtual Location Location { get; set; }
        public virtual WeightType Weight { get; set; }
        public virtual ICollection<ImagePost> ImagePosts { get; set; }
        public virtual ICollection<Love> Loves { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
