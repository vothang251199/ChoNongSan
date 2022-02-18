using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Meet
    {
        public int MeetId { get; set; }
        public int PostId { get; set; }
        public DateTime? Date { get; set; }
        public int? StatusMeet { get; set; }
        public int NguoiTaoLich { get; set; }
        public string Phone { get; set; }

        public virtual Account NguoiTaoLichNavigation { get; set; }
        public virtual Post Post { get; set; }
    }
}
