using ChoNongSan.ViewModels.Requests.TaiKhoan;
using Microsoft.AspNetCore.Http;

namespace ChoNongSan.ApiUsedForWeb.ViewModels
{
    public class ProfileTabVm
    {
        public UpdateAccountRequest Request { get; set; }
        public ChangePassRequest EdiPassRequest { get; set; }
        public TabProfile ActiveTab { get; set; }
        public int accountId { get; set; }
        public string status { get; set; }
    }

    public enum TabProfile
    {
        ThongTin,
        CapNhat,
        DoiMK,
    }
}