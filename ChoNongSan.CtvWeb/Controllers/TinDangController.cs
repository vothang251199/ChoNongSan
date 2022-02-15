using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.CTV;
using ChoNongSan.ViewModels.Requests.TinDang;
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
	public class TinDangController : Controller
	{
		private readonly IPostApi _postApi;
		private readonly IConfiguration _config;

		public TinDangController(IPostApi postApi, IConfiguration config)
		{
			_postApi = postApi;
			_config = config;
		}

		public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 5)
		{
			var request = new FilterPostRequest()
			{
				Keyword = keyword,
				Roles = 2,
				PageIndex = pageIndex,
				PageSize = pageSize
			};
			var data = await _postApi.GetPostPaging(request);
			foreach (var post in data.Items)
			{
				post.ImageDefault = _config["ApiUrl"] + post.ImageDefault;
			}
			ViewBag.Keyword = keyword;
			ViewBag.Link = "/TinDang";
			ViewBag.Obj = data;
			return View(data);
		}

		[HttpGet]
		public async Task<IActionResult> Duyet(int postId)
		{
			var data = await _postApi.GetDetail(postId);
			for (var i = 0; i < data.ListImage.Count; i++)
			{
				data.ListImage[i] = _config["ApiUrl"] + data.ListImage[i];
			}
			return View(data);
		}

		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> Duyet(PostVmChiTiet post)
		{
			if (!ModelState.IsValid)
				return View(post);

			var request = new AcceptPostRequest()
			{
				PostID = post.PostID,
				Reason = post.Reason,
				StatustPost = post.StatusPost
			};
			var data = await _postApi.DuyetTin(request);
			var obj = (JObject)JsonConvert.DeserializeObject(data);
			var status = Convert.ToString(obj["status"]);
			var message = Convert.ToString(obj["message"]);
			TempData["ALertMessage"] = message;
			if (status.Contains("FAILED"))
				return View();

			return RedirectToAction("Index", "TinDang");
		}
	}
}