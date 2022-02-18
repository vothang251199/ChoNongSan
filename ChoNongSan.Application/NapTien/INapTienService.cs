using ChoNongSan.Application.Common.Files;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.NapTien;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.NapTien
{
	public interface INapTienService
	{
		Task<bool> CreateOrUpdate(LichSuNapTienRequest request);

		Task<PageResult<NapTienVm>> GetListNapTien(int accountId, int role, GetPagingCommonRequest request);
	}

	public class NapTienService : INapTienService
	{
		private readonly ChoNongSanContext _context;
		private readonly IStorageService _storageService;
		private readonly IConfiguration _config;
		private const string NAPTIEN_CONTENT_FOLDER_NAME = "naptien-content";

		public NapTienService(ChoNongSanContext context, IStorageService storageService, IConfiguration config)
		{
			_context = context;
			_storageService = storageService;
			_config = config;
		}

		public async Task<bool> CreateOrUpdate(LichSuNapTienRequest request)
		{
			try
			{
				if (request.Role == 3)
				{
					var naptien = new HistoryMoney()
					{
						AccountId = request.AccountId,
						NumberMoney = request.Sotien,
						Time = DateTime.Now,
						CachNap = request.Cachnap,
						Status = 0,
					};
					if (request.Anhnaptien != null)
					{
						naptien.Anh = await this.SaveFile(request.Anhnaptien);
					}
					_context.HistoryMoneys.Add(naptien);
				}
				else
				{
					var his = await _context.HistoryMoneys.FindAsync(request.HisId);
					his.Status = request.Status;
					his.Ctv = request.CTV;

					var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == his.AccountId);
					if (user != null)
					{
						user.MoneyOfOver += his.NumberMoney;
						_context.Accounts.Update(user);
					}
					_context.HistoryMoneys.Update(his);
				}

				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<PageResult<NapTienVm>> GetListNapTien(int accountId, int role, GetPagingCommonRequest request)
		{
			List<HistoryMoney> lsNapTien = null;
			if (role == 3)
			{
				lsNapTien = await (from n in _context.HistoryMoneys.AsNoTracking()
								   where n.AccountId == accountId
								   select n
						   ).ToListAsync();
			}
			else
			{
				lsNapTien = await (from n in _context.HistoryMoneys.AsNoTracking()
								   where n.Status == 0
								   select n
						   ).ToListAsync();
			}

			var totalRow = lsNapTien.Count();
			List<NapTienVm> data;
			if (role == 3)
			{
				if (request.PageIndex != 0 && request.PageSize != 0)
				{
					data = lsNapTien.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)
					.Select(x => new NapTienVm()
					{
						HisId = x.HisId,
						anhnaptien = _config["ApiUrl"] + x.Anh,
						Cachnap = x.CachNap,
						Sotien = (decimal)x.NumberMoney,
						Time = (DateTime)x.Time,
						Status = (int)x.Status,
					}).ToList();
				}
				else
				{
					data = lsNapTien.Select(x => new NapTienVm()
					{
						HisId = x.HisId,
						anhnaptien = _config["ApiUrl"] + x.Anh,
						Cachnap = x.CachNap,
						Sotien = (decimal)x.NumberMoney,
						Time = (DateTime)x.Time,
						Status = (int)x.Status,
						TenNguoiNap = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.AccountId).FullName,
					}).ToList();
				}
			}
			else
			{
				if (request.PageIndex != 0 && request.PageSize != 0)
				{
					data = lsNapTien.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)
					.Select(x => new NapTienVm()
					{
						HisId = x.HisId,
						Cachnap = x.CachNap,
						Sotien = (decimal)x.NumberMoney,
						Time = (DateTime)x.Time,
						Status = (int)x.Status,
						anhnaptien = _config["ApiUrl"] + x.Anh,
						TenNguoiNap = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.AccountId).FullName,
					}).ToList();
				}
				else
				{
					data = lsNapTien.Select(x => new NapTienVm()
					{
						HisId = x.HisId,
						Cachnap = x.CachNap,
						Sotien = (decimal)x.NumberMoney,
						Time = (DateTime)x.Time,
						Status = (int)x.Status,
						anhnaptien = _config["ApiUrl"] + x.Anh,
						TenNguoiNap = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountId == x.AccountId).FullName,
					}).ToList();
				}
			}

			var result = new PageResult<NapTienVm>()
			{
				Items = data,
				PageIndex = request.PageIndex,
				PageSize = request.PageSize,
				TotalRecords = totalRow,
			};

			return result;
		}

		private async Task<string> SaveFile(IFormFile file)
		{
			var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
			await _storageService.SaveFileAsync(file.OpenReadStream(), fileName, "naptien");
			return "/" + NAPTIEN_CONTENT_FOLDER_NAME + "/" + fileName;
		}
	}
}