using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostApi _postApi;
        private readonly IConfiguration _config;
        private readonly ICategoryApi _categoryApi;

        public HomeController(ILogger<HomeController> logger, IPostApi postApi, IConfiguration config,
            ICategoryApi categoryApi)
        {
            _categoryApi = categoryApi;
            _logger = logger;
            _postApi = postApi;
            _config = config;
        }

        public async Task<IActionResult> Index(string keyword, string tabname, int pageIndex = 1, int pageSize = 10)
        {
            HomeTabVm vm = new HomeTabVm();
            
            GetPagingCommonRequest request = new GetPagingCommonRequest();

            request.ById = Convert.ToInt32(tabname);

            request.Keyword = keyword;
            request.PageIndex = pageIndex;
            request.PageSize = pageSize;

            var data = await _postApi.GetPostPaging(request);
            vm.Data = data;

            var lsCat = await _categoryApi.GetListCat();

            foreach (var i in lsCat)
            {
                i.Image = _config["ApiUrl"] + i.Image;
            }

            vm.ListCat = lsCat;
            vm.ActiveTab =(int) request.ById;

            ViewBag.Keyword = keyword;
            ViewBag.ApiUrl = _config["ApiUrl"];
            return View(vm);
        }

        public async Task<IActionResult> SwitchTabs(string tabname)
        {
            var vm = new HomeTabVm();
            var lsCat = await _categoryApi.GetListCat();
            foreach (var i in lsCat)
            {
                if (Convert.ToInt32(tabname) == i.CategoryID)
                {
                    vm.ActiveTab = i.CategoryID;
                }
                else
                {
                    vm.ActiveTab = 0;
                }
            }

            return RedirectToAction(nameof(QuanLyTinDang.Index), vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}