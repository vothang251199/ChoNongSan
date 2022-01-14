using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.ViewModels.Requests.TaiKhoan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers.Components
{
    public class CapNhatViewComponent : ViewComponent
    {
        private readonly IUserApi _userApi;
        private readonly IConfiguration _config;

        public CapNhatViewComponent(IUserApi userApi, IConfiguration config)
        {
            _userApi = userApi;
            _config = config;
        }

        public async Task<IViewComponentResult> InvokeAsync(ProfileTabVm vm)
        {
            var user = await _userApi.GetUserById(vm.accountId);

            if (vm.status != null)
            {
                if (string.IsNullOrEmpty(vm.Request.Email))
                    ModelState.AddModelError("Email", "Nhập Emai");
                if (string.IsNullOrEmpty(vm.Request.PhoneNumber))
                    ModelState.AddModelError("PhoneNumber", "Nhập số điện thoại");
                if (string.IsNullOrEmpty(vm.Request.FullName))
                    ModelState.AddModelError("FullName", "Nhập họ và tên");
                if (vm.Request.ThumbnailImage == null)
                {
                    ViewBag.Avatar = _config["ApiUrl"] + user.Avatar;
                }
                return await Task.FromResult((IViewComponentResult)View("CapNhat", vm.Request));
            }
            else
            {
                UpdateAccountRequest request = new UpdateAccountRequest();
                request.AccountID = user.AccountId;
                request.Address = user.Address;
                request.Email = user.Email;
                request.FullName = user.FullName;
                request.PhoneNumber = user.PhoneNumber;
                ViewBag.Avatar = _config["ApiUrl"] + user.Avatar;
                ViewBag.Id = user.AccountId;
                return await Task.FromResult((IViewComponentResult)View("CapNhat", request));
            }
        }
    }
}