using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Banner
    {
        public int BannerId { get; set; }
        public string ImagePath { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Topic { get; set; }
        public bool? IsDelete { get; set; }
    }
}
