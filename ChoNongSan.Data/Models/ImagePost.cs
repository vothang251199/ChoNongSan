using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class ImagePost
    {
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
        public int PostId { get; set; }
        public bool? IsDefault { get; set; }

        public virtual Post Post { get; set; }
    }
}
