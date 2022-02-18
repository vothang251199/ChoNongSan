using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.DanhGia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.DanhGia
{
	public interface IReviewService
	{
		Task<bool> Create(ReviewRequest request);
	}

	public class ReviewService : IReviewService
	{
		private readonly ChoNongSanContext _context;

		public ReviewService(ChoNongSanContext context)
		{
			_context = context;
		}

		public async Task<bool> Create(ReviewRequest request)
		{
			try
			{
				var danhgia = new Review()
				{
					AccountId = request.AccountId,
					PostId = request.PostId,
					Contents = request.Noidung,
					NumberOfReviews = request.Sao,
					Time = DateTime.Now
				};

				_context.Reviews.Add(danhgia);
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