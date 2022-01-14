using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Love
    {
        public int PostId { get; set; }
        public int AccountId { get; set; }
        public DateTime? AddTime { get; set; }

        public virtual Account Account { get; set; }
        public virtual Post Post { get; set; }
    }
}
