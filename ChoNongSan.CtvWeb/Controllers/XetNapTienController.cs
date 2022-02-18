using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.NapTien;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.CtvWeb.Controllers
{
	[Authorize]
	public class XetNapTienController : Controller
	{
		private readonly INapTienApi _napTienApi;
		private readonly IConfiguration _config;

		public XetNapTienController(IConfiguration config, INapTienApi napTienApi)
		{
			_napTienApi = napTienApi;
			_config = config;
		}

		public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 5)
		{
			var request = new GetPagingCommonRequest()
			{
				Keyword = keyword,
				Roles = 2,
				PageIndex = pageIndex,
				PageSize = pageSize
			};
			var UserId = Convert.ToInt32(User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault());
			ViewBag.CTV = UserId;
			var data = await _napTienApi.GetListNapTien(UserId, 2, request);
			var obj = (JObject)JsonConvert.DeserializeObject(data);

			var result = obj["data"].ToObject<PageResult<NapTienVm>>();
			ViewBag.Keyword = keyword;
			ViewBag.Link = "/XetNapTien";
			ViewBag.Obj = result;
			return View(result);
		}
		[HttpPost]
		public async Task<JsonResult> Duyet(LichSuNapTienRequest request)
		{
			var data = await _napTienApi.CreateOrUpdate(request);
			return Json(data);
		}
	}
}