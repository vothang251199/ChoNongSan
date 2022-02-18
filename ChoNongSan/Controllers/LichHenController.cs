using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
	[Authorize]
	public class LichHenController : Controller
	{
		private readonly IMeetApi _meetApi;

		public LichHenController(IMeetApi meetApi)
		{
			_meetApi = meetApi;
		}

		public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 8)
		{
			var userId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();

			var request = new GetPagingCommonRequest()
			{
				PageIndex = pageIndex,
				PageSize = pageSize
			};
			var data = await _meetApi.GetListMeet(Convert.ToInt32(userId), request);
			var obj = (JObject)JsonConvert.DeserializeObject(data);
			if (Convert.ToString(obj["status"]).Contains("FAILED"))
			{
				var message = Convert.ToString(obj["message"]);
				TempData["ALertMessage"] = message;
				return View();
			}
			var result = obj["data"].ToObject<PageResult<MeetVm>>();
			return View(result);
		}

		[HttpGet]
		public async Task<JsonResult> DuyetLich(int meetId, int stt)
		{
			var data = await _meetApi.DuyetLich(meetId, stt);

			return Json(data);
		}
	}
}