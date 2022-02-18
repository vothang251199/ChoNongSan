using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class HistoryMoney
    {
        public int HisId { get; set; }
        public DateTime? Time { get; set; }
        public decimal? NumberMoney { get; set; }
        public int AccountId { get; set; }
        public int? Ctv { get; set; }
        public int? Status { get; set; }
        public string CachNap { get; set; }
        public string Anh { get; set; }

        public virtual Account Account { get; set; }
    }
}
