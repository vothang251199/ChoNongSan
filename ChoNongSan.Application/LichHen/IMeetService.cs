using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.LichHen;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.LichHen
{
	public interface IMeetService
	{
		Task<bool> Create(CreateMeetRequest request);
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
	}
}