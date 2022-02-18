using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.LichHen;
using ChoNongSan.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.LichHen
{
	public interface IMeetService
	{
		Task<bool> Create(CreateMeetRequest request);

		Task<PageResult<MeetVm>> GetListMeet(int accountId, GetPagingCommonRequest request);

		Task<bool> DuyetLich(int meetId, int stt);
	}

	public class MeetService : IMeetService
	{
		private readonly ChoNongSanContext _context;

		public MeetService(ChoNongSanContext context)
		{
			_context = context;
		}

		public async Task<bool> Create(CreateMeetRequest request)
		{
			try
			{
				var meet = new Meet()
				{
					PostId = request.PostId,
					Date = DateTime.Parse(request.Date + " " + request.Time),
					NguoiTaoLich = request.NguoiTaoLich,
					Phone = request.Phone,
					StatusMeet = 0,
				};

				_context.Meets.Add(meet);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<PageResult<MeetVm>> GetListMeet(int accountId, GetPagingCommonRequest request)
		{
			var lsMeeet = await (from m in _context.Meets.AsNoTracking()
								 join p in _context.Posts.AsNoTracking().Where(x => x.AccountId == accountId)
								 on m.PostId equals p.PostId
								 select m
						   ).ToListAsync();
			var totalRow = lsMeeet.Count;
			List<MeetVm> data;
			if (request.PageIndex != 0 && request.PageSize != 0)
			{
				data = lsMeeet.Skip((request.PageIndex - 1) * request.PageSize)
				.Take(request.PageSize)
				.Select(x => new MeetVm()
				{
					Title = _context.Posts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId).Title,
					PhoneNumber = x.Phone,
					StatusMeet = (int)x.StatusMeet,
					TenNguoiTaoLich = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.NguoiTaoLich).FullName,
					ThoiGian = (DateTime)x.Date,
					MeetId = x.MeetId
				}).ToList();
			}
			else
			{
				data = lsMeeet.Select(x => new MeetVm()
				{
					Title = _context.Posts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId).Title,
					PhoneNumber = x.Phone,
					StatusMeet = (int)x.StatusMeet,
					TenNguoiTaoLich = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.NguoiTaoLich).FullName,
					ThoiGian = (DateTime)x.Date,
					MeetId = x.MeetId
				}).ToList();
			}

			var result = new PageResult<MeetVm>()
			{
				Items = data,
				PageIndex = request.PageIndex,
				PageSize = request.PageSize,
				TotalRecords = totalRow,
			};

			return result;
		}

		public async Task<bool> DuyetLich(int meetId, int stt)
		{
			try
			{
				var meet = await _context.Meets.FindAsync(meetId);
				meet.StatusMeet = stt;
				_context.Meets.Update(meet);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}