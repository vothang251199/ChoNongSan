using ChoNongSan.Application.NapTien;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.NapTien;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
	[Route("api/nap-tien")]
	[ApiController]
	public class NapTiensController : ControllerBase
	{
		private readonly INapTienService _napTienService;
		private readonly ChoNongSanContext _context;

		public NapTiensController(INapTienService napTienService, ChoNongSanContext context)
		{
			_napTienService = napTienService;
			_context = context;
		}

		[HttpPost("tao-or-cap-nhat")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateOrUpdate([FromForm] LichSuNapTienRequest request)
		{
			if (request.Role == 3)
			{
				var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == request.AccountId && x.IsDelete == false);
				if (user != null)
				{
					var result = await _napTienService.CreateOrUpdate(request);
					if (result)
						return Ok(new { message = "Tạo xác nhận thành công. Chờ CTV duyệt", status = "OK" });
				}
				return BadRequest(new { message = "Tài khoản không tồn tại", status = "FAILED" });
			}
			else if (request.Role == 2)
			{
				var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == request.CTV && x.IsDelete == false);
				var his = await _context.HistoryMoneys.AsNoTracking().FirstOrDefaultAsync(x => x.HisId == request.HisId);
				if (user != null)
				{
					if (his != null)
					{
						var result = await _napTienService.CreateOrUpdate(request);
						if (result)
							return Ok(new { message = "Đã cập nhật trạng thái nạp tiền", status = "OK" });
					}
					return BadRequest(new { message = "Lịch sử nạp tiền không tồn tại", status = "FAILED" });
				}
				return BadRequest(new { message = "Tài khoản không tồn tại", status = "FAILED" });
			}
			return BadRequest(new { message = "Lỗi Api", status = "FAILED" });
		}

		[HttpGet("danh-sach-nap-tien/{accountId}/{role}")]
		public async Task<IActionResult> GetListMeet([FromRoute] int accountId, [FromRoute] int role, [FromQuery] GetPagingCommonRequest request)
		{
			var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (user != null && (role == 2 || role == 3))
			{
				return Ok(new { data = await _napTienService.GetListNapTien(accountId, role, request), status = "OK" });
			}
			return BadRequest(new { message = "Thất bại", status = "FAILED" });
		}
	}
}