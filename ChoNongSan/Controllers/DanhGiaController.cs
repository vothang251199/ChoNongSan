using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ViewModels.Requests.DanhGia;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
	public class DanhGiaController : Controller
	{
		private readonly IReviewApi _reviewApi;

		public DanhGiaController(IReviewApi reviewApi)
		{
			_reviewApi = reviewApi;
		}

		[HttpPost]
		public async Task<JsonResult> CreateReview([FromBody] ReviewRequest request)
		{
			var data = await _reviewApi.Create(request);

			return Json(data);
		}
	}
}