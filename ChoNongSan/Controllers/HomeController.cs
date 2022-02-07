using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
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
		private readonly IUserApi _userApi;

		public HomeController(ILogger<HomeController> logger, IPostApi postApi, IConfiguration config,
			ICategoryApi categoryApi, IUserApi userApi)
		{
			_userApi = userApi;
			_categoryApi = categoryApi;
			_logger = logger;
			_postApi = postApi;
			_config = config;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string keyword, string tabname, int pageIndex = 1, int pageSize = 9)
		{
			HomeTabVm vm = new();

			vm.keyword = keyword;
			var request = new GetPagingCommonRequest
			{
				ById = Convert.ToInt32(tabname),
				Roles = 3,
				Keyword = keyword,
				PageIndex = pageIndex,
				PageSize = pageSize
			};

			var data = await _postApi.GetPostPaging(request);
			foreach (var i in data.Items)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			vm.Data = data;

			var lsCat = await _categoryApi.GetListCat();

			foreach (var i in lsCat)
			{
				i.Image = _config["ApiUrl"] + i.Image;
			}

			vm.ListCat = lsCat;
			vm.ActiveTab = (int)request.ById;

			var lsMany = await _postApi.ListManyViews(6);
			foreach (var i in lsMany)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			vm.ListManyViews = lsMany;

			var lsNew = await _postApi.ListPostNew(3);
			foreach (var i in lsNew)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			vm.ListPostNew = lsNew;

			var id = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
			if (string.IsNullOrEmpty(id))
				ViewBag.AccountId = 0;
			else
				ViewBag.AccountId = id;

			ViewBag.Keyword = keyword;
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Index(HomeTabVm vm)
		{
			ViewBag.Keyword = vm.keyword;
			var rq = new GetPagingCommonRequest
			{
				ById = 0,
				Roles = 3,
				Keyword = "",
				PageIndex = 1,
				PageSize = 9,
				RequestFilterPost = vm.RequestFilterPost
			};

			var data = await _postApi.GetPostPaging(rq);
			foreach (var i in data.Items)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			vm.Data = data;

			var lsCat = await _categoryApi.GetListCat();

			foreach (var i in lsCat)
			{
				i.Image = _config["ApiUrl"] + i.Image;
			}

			vm.ListCat = lsCat;
			vm.ActiveTab = vm.RequestFilterPost.CategoryId;

			var lsMany = await _postApi.ListManyViews(6);
			foreach (var i in lsMany)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			vm.ListManyViews = lsMany;

			var lsNew = await _postApi.ListPostNew(3);
			foreach (var i in lsNew)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			vm.ListPostNew = lsNew;

			var id = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
			if (string.IsNullOrEmpty(id))
				ViewBag.AccountId = 0;
			else
				ViewBag.AccountId = id;

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