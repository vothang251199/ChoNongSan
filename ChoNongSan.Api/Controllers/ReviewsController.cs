using ChoNongSan.Application.DanhGia;
using ChoNongSan.Application.LichHen;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.DanhGia;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
	[Route("api/danhgia")]
	[ApiController]
	public class ReviewsController : ControllerBase
	{
		private readonly IReviewService _reviewService;
		private readonly ChoNongSanContext _context;

		public ReviewsController(IReviewService reviewService, ChoNongSanContext context)
		{
			_reviewService = reviewService;
			_context = context;
		}

		[HttpPost("tao-danh-gia")]
		public async Task<IActionResult> Create([FromBody] ReviewRequest request)
		{
			var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.PostId == request.PostId && x.StatusPost == 2);
			if (post != null)
			{
				var user = await _context.Accounts.FindAsync(request.AccountId);
				if (user != null)
				{
					var meet = await _context.Meets.AsNoTracking().FirstOrDefaultAsync(x => x.PostId == request.PostId && x.NguoiTaoLich == request.AccountId && x.StatusMeet == 2);
					if (meet != null)
					{
						var danhgia = await _context.Reviews.AsNoTracking().FirstOrDefaultAsync(x => x.PostId == request.PostId && x.AccountId == request.AccountId);
						if (danhgia == null)
						{
							var result = await _reviewService.Create(request);
							if (result)
								return Ok(new { message = "Đánh giá thành công", status = "OK" });
						}
						else
						{
							return BadRequest(new { message = "Bạn đã đánh giá rồi", status = "FAILED" });
						}
					}
					else
					{
						return BadRequest(new { message = "Vui lòng tạo lịch hẹn để được đánh giá", status = "FAILED" });
					}
				}
			}
			return BadRequest(new { message = "Đánh giá thất bại", status = "FAILED" });
		}
	}
}