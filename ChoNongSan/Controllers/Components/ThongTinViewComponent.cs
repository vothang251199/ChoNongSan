using ChoNongSan.ApiUsedForWeb.ApiService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers.Components.Profiles
{
    public class ThongTinViewComponent : ViewComponent
    {
        private readonly IUserApi _userApi;
        private readonly IConfiguration _config;

        public ThongTinViewComponent(IUserApi userApi, IConfiguration config)
        {
            _userApi = userApi;
            _config = config;
        }

        public async Task<IViewComponentResult> InvokeAsync(int accountID)
        {
            var model = await _userApi.GetUserById(accountID);
            model.Avatar = _config["ApiUrl"] + model.Avatar;
            return await Task.FromResult((IViewComponentResult)View("ThongTin", model));
        }
    }
}