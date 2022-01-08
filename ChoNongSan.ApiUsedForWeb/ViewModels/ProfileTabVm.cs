using ChoNongSan.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ViewModels
{
    public class ProfileTabVm
    {
        public TabProfile ActiveTab { get; set; }
        public int accountId { get; set; }
    }

    public enum TabProfile
    {
        Display,
        Love,
        Update,
        ChangePass,
    }
}