using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhGia;
using ChoNongSan.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

		Task<PageResult<ReviewVm>> GetListReview(int accountId, GetPagingCommonRequest request);
	}

	public class ReviewService : IReviewService
	{
		private readonly ChoNongSanContext _context;
		private readonly IConfiguration _config;

		public ReviewService(ChoNongSanContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
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

		public async Task<PageResult<ReviewVm>> GetListReview(int postId, GetPagingCommonRequest request)
		{
			var lsMeeet = await _context.Reviews.AsNoTracking().Where(x => x.PostId == postId).ToListAsync();
			var totalRow = lsMeeet.Count;
			List<ReviewVm> data;
			if (request.PageIndex != 0 && request.PageSize != 0)
			{
				data = lsMeeet.Skip((request.PageIndex - 1) * request.PageSize)
				.Take(request.PageSize)
				.Select(x => new ReviewVm()
				{
					ReviewsId = x.ReviewsId,
					PostId = x.PostId,
					Avatar = _config["ApiUrl"] + _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.AccountId).Avatar,
					Contents = x.Contents,
					Name = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.AccountId).FullName,
					NumberOfReviews = x.NumberOfReviews,
					Time = x.Time
				}).ToList();
			}
			else
			{
				data = lsMeeet.Select(x => new ReviewVm()
				{
					ReviewsId = x.ReviewsId,
					PostId = x.PostId,
					Avatar = _config["ApiUrl"] +  _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.AccountId).Avatar,
					Contents = x.Contents,
					Name = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.AccountId).FullName,
					NumberOfReviews = x.NumberOfReviews,
					Time = x.Time
				}).ToList();
			}

			var result = new PageResult<ReviewVm>()
			{
				Items = data,
				PageIndex = request.PageIndex,
				PageSize = request.PageSize,
				TotalRecords = totalRow,
			};

			return result;
		}
	}
}