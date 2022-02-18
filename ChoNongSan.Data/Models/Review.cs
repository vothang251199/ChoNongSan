using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Review
    {
        public int ReviewsId { get; set; }
        public int PostId { get; set; }
        public int AccountId { get; set; }
        public string Contents { get; set; }
        public int? NumberOfReviews { get; set; }
        public DateTime? Time { get; set; }

        public virtual Account Account { get; set; }
        public virtual Post Post { get; set; }
    }
}
