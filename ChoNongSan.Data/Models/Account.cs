using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Account
    {
        public Account()
        {
            HistoryMoneyAccounts = new HashSet<HistoryMoney>();
            HistoryMoneyWhoAddMoneyNavigations = new HashSet<HistoryMoney>();
            Loves = new HashSet<Love>();
            Meets = new HashSet<Meet>();
            Posts = new HashSet<Post>();
            Reviews = new HashSet<Review>();
        }

        public int AccountId { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string KeySecurity { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public int? NumberOfPost { get; set; }
        public decimal? MoneyOfOver { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsDelete { get; set; }
        public int RolesId { get; set; }

        public virtual Role Roles { get; set; }
        public virtual ICollection<HistoryMoney> HistoryMoneyAccounts { get; set; }
        public virtual ICollection<HistoryMoney> HistoryMoneyWhoAddMoneyNavigations { get; set; }
        public virtual ICollection<Love> Loves { get; set; }
        public virtual ICollection<Meet> Meets { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
