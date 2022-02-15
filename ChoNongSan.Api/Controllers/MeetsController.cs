using ChoNongSan.Application.LichHen;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.LichHen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
	[Route("api/meet")]
	[ApiController]
	public class MeetsController : ControllerBase
	{
		private readonly IMeetService _meetService;
		private readonly ChoNongSanContext _context;

		public MeetsController(IMeetService meetService, ChoNongSanContext context)
		{
			_meetService = meetService;
			_context = context;
		}

		[HttpPost("tao-lich-hen")]
		public async Task<IActionResult> Create([FromBody] CreateMeetRequest request)
		{
			var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.PostId == request.PostId && x.StatusPost == 2);
			if (post != null)
			{
				var user = await _context.Accounts.FindAsync(request.NguoiTaoLich);
				var userPost = await _context.Accounts.FindAsync(post.AccountId);
				if (user != null)
				{
					if (user.AccountId != userPost.AccountId)
					{
						var result = await _meetService.Create(request);
						if (result)
							return Ok(new { message = "Tạo lịch hẹn thành công", status = "OK" });
					}
					else
					{
						return BadRequest(new { message = "Đây là tin đăng của bạn, không cần tạo lịch hẹn", status = "FAILED" });
					}
				}
			}
			return BadRequest(new { message = "Tạo lịch hẹn thất bại", status = "FAILED" });
		}

		[HttpGet("check-meet/{postId}/{accountId}")]
		public async Task<IActionResult> CheckMeet([FromRoute] int postId, [FromRoute] int accountId)
		{
			//api này để kiểm tra xem người dùng đã có lịch hẹn với người đăng người viết hay chưa.
			var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.PostId == postId && x.StatusPost == 2);
			if (post != null)
			{
				var user = await _context.Accounts.FindAsync(accountId);
				if (user != null)
				{
					var meet = await _context.Meets.AsNoTracking().FirstOrDefaultAsync(x => x.PostId == postId && x.NguoiTaoLich == accountId);

					if (meet != null)
						return Ok(new { status = "OK" });
				}
			}
			return BadRequest(new { status = "FAILED" });
		}
	}
}