using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.Application.Common.Files;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhGia;
using ChoNongSan.ViewModels.Requests.LichHen;
using ChoNongSan.ViewModels.Requests.TinDang;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
	[Authorize]
	public class QuanLyTinDang : Controller
	{
		private readonly IConfiguration _config;
		private readonly IPostApi _postApi;
		private readonly IStorageService _storageService;
		private readonly ICategoryApi _categoryApi;
		private readonly IWeightApi _weightApi;
		private readonly IUserApi _userApi;
		private readonly IMeetApi _meetApi;

		public QuanLyTinDang(IConfiguration config, IPostApi postApi, IStorageService storageService,
			ICategoryApi categoryApi, IWeightApi weightApi, IUserApi userApi, IMeetApi meetApi)
		{
			_userApi = userApi;
			_weightApi = weightApi;
			_categoryApi = categoryApi;
			_storageService = storageService;
			_config = config;
			_postApi = postApi;
			_meetApi = meetApi;
		}

		[HttpGet]
		public async Task<IActionResult> Index(PostTabVm vm, int pageIndex = 1, int pageSize = 5)
		{
			if (vm == null)
			{
				vm = new PostTabVm
				{
					ActiveTab = Tab.HienThi,
				};
			}

			if (vm.ActiveTab == Tab.HienThi)
			{
				vm.byId = 2;
			}

			var request = new GetPagingCommonRequest()
			{
				ById = vm.byId,
				PageIndex = pageIndex,
				PageSize = pageSize,
			};

			var accountId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();

			var data = await _postApi.GetAllByStatusPaging(Convert.ToInt32(accountId), request);
			foreach (var i in data.Items)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			vm.Data = data;
			vm.pageIndex = pageIndex;
			vm.pageSize = pageSize;

			return View(vm);
		}

		public IActionResult SwitchTabs(string tabname)
		{
			var vm = new PostTabVm();
			switch (tabname)
			{
				case "HienThi":
					vm.ActiveTab = Tab.HienThi;
					vm.byId = 2;
					break;

				case "DoiDuyet":
					vm.ActiveTab = Tab.DoiDuyet;
					vm.byId = 0;
					break;

				case "TuChoi":
					vm.ActiveTab = Tab.TuChoi;
					vm.byId = 1;
					break;

				case "DaAn":
					vm.ActiveTab = Tab.DaAn;
					vm.byId = 3;
					break;

				default:
					vm.ActiveTab = Tab.HienThi;
					vm.byId = 2;
					break;
			}
			return RedirectToAction(nameof(QuanLyTinDang.Index), vm);
		}

		[HttpGet]
		public async Task<IActionResult> TaoMoi()
		{
			ViewBag.ApiUrl = _config["ApiUrl"];
			ViewBag.CategoryList = await _categoryApi.GetListCat();
			ViewBag.WeightList = await _weightApi.GetListWeight();

			var userId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
			var user = await _userApi.GetUserById(Convert.ToInt32(userId));
			ViewBag.Phone = user.PhoneNumber;

			return View();
		}

		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> TaoMoi(CreatePostRequest request)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.CategoryList = await _categoryApi.GetListCat();
				ViewBag.WeightList = await _weightApi.GetListWeight();
				var userId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
				var user = await _userApi.GetUserById(Convert.ToInt32(userId));
				ViewBag.Phone = user.PhoneNumber;
				ViewBag.LsImg = request.ThumbnailImage;
				ViewBag.ApiUrl = _config["ApiUrl"];
				ViewBag.LsDistrict = await _postApi.GetListDistrict(Convert.ToInt32(request.Province));
				ViewBag.LsSubDistrict = await _postApi.GetListSubDistrict(Convert.ToInt32(request.Province), Convert.ToInt32(request.District));
				return View(request);
			}

			var result = await _postApi.CreatePost(request);
			if (!result)
			{
				TempData["ALertMessage"] = "Đăng tin không thành công";
				return View();
			}
			TempData["ALertMessage"] = "Đăng tin thành công, chờ hệ thống xét duyệt";
			var vm = new PostTabVm
			{
				ActiveTab = Tab.DoiDuyet
			};

			return RedirectToAction(nameof(QuanLyTinDang.Index), vm);
		}

		[HttpGet]
		public async Task<IActionResult> YeuThich(int pageIndex = 1, int pageSize = 4)
		{
			var userId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
			GetPagingCommonRequest request = new GetPagingCommonRequest();

			request.ById = Convert.ToInt32(userId);
			request.PageIndex = pageIndex;
			request.PageSize = pageSize;

			var result = await _postApi.GetAllLoveByAccountId(request);
			var obj = (JObject)JsonConvert.DeserializeObject(result);
			var status = Convert.ToString(obj["status"]);
			var model = obj["data"].ToObject<PageResult<PostVmTongQuat>>();
			foreach (var i in model.Items)
			{
				i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> AddlovePost([FromBody] string postId)
		{
			var accountId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
			var request = new LoveRequest()
			{
				accountId = Convert.ToInt32(accountId),
				postId = Convert.ToInt32(postId)
			};
			var data = await _postApi.AddPostLove(request);

			return Json(data);
		}

		[HttpPost]
		public async Task<JsonResult> GetListDistrict([FromBody] int codeProvince)
		{
			var data = await _postApi.GetListDistrict(codeProvince);

			return Json(data);
		}

		[HttpGet]
		public async Task<JsonResult> GetListSubDistrict(int codePro, int codeDis)
		{
			var data = await _postApi.GetListSubDistrict(Convert.ToInt32(codePro), Convert.ToInt32(codeDis));

			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> ChiTiet(int postId)
		{
			var model = await _postApi.GetDetail(postId);
			model.Avatar = _config["ApiUrl"] + model.Avatar;
			for (var i = 0; i < model.ListImage.Count; i++)
			{
				model.ListImage[i] = _config["ApiUrl"] + model.ListImage[i];
			}

			var a = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
			if (string.IsNullOrEmpty(a))
				ViewBag.AccountId = 0;
			else
				ViewBag.AccountId = a;

			ViewBag.HiddenLayOut = 1;

			var check = await _meetApi.CheckMeet(postId, Convert.ToInt32(a));
			var obj = (JObject)JsonConvert.DeserializeObject(check);
			var status = Convert.ToString(obj["status"]);
			if (model.AccountId != Convert.ToInt32(a))
			{
				if (status.Contains("Yes"))
				{
					ViewBag.Check = 1;
				}
				else
				{
					ViewBag.Check = 0;
				}
			}
			else
			{
				ViewBag.Check = 2;
			}

			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> CreateMeet([FromBody] CreateMeetRequest request)
		{
			var data = await _meetApi.Create(request);

			return Json(data);
		}

		[HttpPost]
		public async Task<JsonResult> HiddenPost([FromBody] int postId)
		{
			var data = await _postApi.HiddenPost(postId);
			return Json(data);
		}
	}
}