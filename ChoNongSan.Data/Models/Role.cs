using System;
using System.Collections.Generic;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        public int RolesId { get; set; }
        public string RolesName { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
