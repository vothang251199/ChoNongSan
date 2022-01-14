using ChoNongSan.ViewModels.Requests.TaiKhoan;

namespace ChoNongSan.ApiUsedForWeb.ViewModels
{
    public class ProfileTabVm
    {
        public UpdateAccountRequest Request { get; set; }
        public TabProfile ActiveTab { get; set; }
        public int accountId { get; set; }
        public string status { get; set; }
    }

    public enum TabProfile
    {
        ThongTin,
        YeuThich,
        CapNhat,
        DoiMK,
    }
}