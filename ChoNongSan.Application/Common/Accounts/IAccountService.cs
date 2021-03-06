using ChoNongSan.Application.Common.Files;
using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Extenstions;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TaiKhoan;
using ChoNongSan.ViewModels.Requests.TaiKhoan.KhachHang;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common.Accounts
{
	public interface IAccountService
	{
		Task<PageResult<AccountVm>> GetAll(GetPagingCommonRequest request);

		Task<AccountVm> GetAccountById(int accountID);

		Task<int> Register(RegisterRequest request);

		Task<LoginVm> Login(LoginRequest request);

		Task<int> Update(UpdateAccountRequest request);

		Task<bool> Delete(int AccountID);

		Task<int> ChangePassword(ChangePassRequest request);

		string ForgotPassword(Account user);

		Task<string> ResetPassword(ResetPassRequest request);

		Task<Account> GetAccountByPhone(string phone);
	}

	public class AccountService : IAccountService
	{
		private readonly ChoNongSanContext _context;
		private readonly IConfiguration _config;
		private readonly IStorageService _storageService;
		private readonly IMailService _mailService;
		private const string USER_CONTENT_FOLDER_NAME = "user-content";

		public AccountService(ChoNongSanContext context, IStorageService storageService,
			IConfiguration config, IMailService mailService)
		{
			_storageService = storageService;
			_context = context;
			_config = config;
			_mailService = mailService;
		}

		public async Task<int> ChangePassword(ChangePassRequest request)
		{
			string salt = Utilities.Helpper.Utilities.GetRandomKey();

			var user = await _context.Accounts.FindAsync(request.AccountID);

			user.Password = (request.NewPass + salt.Trim()).ToMD5();
			user.KeySecurity = salt;

			_context.Accounts.Update(user);
			return await _context.SaveChangesAsync();
		}

		public async Task<bool> Delete(int AccountID)
		{
			try
			{
				var user = await _context.Accounts.FindAsync(AccountID);
				user.IsDelete = true;

				_context.Accounts.Update(user);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<PageResult<AccountVm>> GetAll(GetPagingCommonRequest request)
		{
			var lsUser = await _context.Accounts.Where(x => x.IsDelete == false).ToListAsync();
			if (!String.IsNullOrEmpty(request.Keyword))
			{
				request.Keyword = request.Keyword.ToLower();
				lsUser = lsUser.Where(x => x.FullName.Contains(request.Keyword) || x.UserName.Contains(request.Keyword)
				 || x.PhoneNumber.Contains(request.Keyword) || x.Email.Contains(request.Keyword)).ToList();
			}

			if (request.ById != null && request.ById != 0)
			{
				lsUser = lsUser.Where(x => x.RolesId == request.ById).ToList();
			}

			var totalRow = lsUser.Count;
			List<AccountVm> data;
			if (request.PageIndex != 0 && request.PageSize != 0)
			{
				data = lsUser.Skip((request.PageIndex - 1) * request.PageSize)
				.Take(request.PageSize)
				.Select(x => new AccountVm()
				{
					AccountId = x.AccountId,
					FullName = x.FullName,
					UserName = x.UserName,
					Address = x.Address,
					Avatar = x.Avatar,
					CreateDate = x.CreateDate,
					Email = x.Email,
					NumberOfPost = x.NumberOfPost,
					PhoneNumber = x.PhoneNumber,
				}).ToList();
			}
			else
			{
				data = lsUser.Select(x => new AccountVm()
				{
					AccountId = x.AccountId,
					FullName = x.FullName,
					UserName = x.UserName,
					Address = x.Address,
					Avatar = x.Avatar,
					CreateDate = x.CreateDate,
					Email = x.Email,
					NumberOfPost = x.NumberOfPost,
					PhoneNumber = x.PhoneNumber,
				}).ToList();
			}

			var pageResult = new PageResult<AccountVm>()
			{
				TotalRecords = totalRow,
				Items = data,
				PageIndex = request.PageIndex,
				PageSize = request.PageSize
			};
			return pageResult;
		}

		public async Task<LoginVm> Login(LoginRequest request)
		{
			var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserName.Contains(request.LoginName.ToLower())
				|| x.PhoneNumber.Contains(request.LoginName) || x.Email.Contains(request.LoginName.ToLower()));

			var claims = new[]
			{
				new Claim(ClaimTypes.Name, request.LoginName.ToLower()),
				new Claim(ClaimTypes.Role, Convert.ToString(user.RolesId)),
				new Claim("Id", Convert.ToString(user.AccountId)),
				new Claim("FullName", user.FullName),
				new Claim("UserName", user.UserName),
				new Claim("Avatar", _config["ApiUrl"] +  user.Avatar),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(_config["Tokens:Issuer"],
			   _config["Tokens:Issuer"],
			   claims,
			   expires: DateTime.Now.AddHours(3),
			   signingCredentials: creds);

			var result = new LoginVm()
			{
				account = user,
				token = new JwtSecurityTokenHandler().WriteToken(token),
			};

			return result;
		}

		public async Task<int> Register(RegisterRequest request)
		{
			string salt = Utilities.Helpper.Utilities.GetRandomKey();

			var user = new Account()
			{
				FullName = request.FullName,
				UserName = request.UserName.ToLower(),
				PhoneNumber = request.PhoneNumber,
				Email = request.Email.ToLower(),
				Password = (request.Password + salt.Trim()).ToMD5(),
				KeySecurity = salt,
				IsDelete = false,
				CreateDate = DateTime.Now,
				Avatar = "/user-content/avatar-null.png",
				RolesId = 3,
				MoneyOfOver = 0
			};

			_context.Accounts.Add(user);

			await _context.SaveChangesAsync();
			return user.AccountId;
		}

		public async Task<int> Update(UpdateAccountRequest request)
		{
			var user = await _context.Accounts.FindAsync(request.AccountID);

			user.PhoneNumber = request.PhoneNumber;
			user.Address = (string.IsNullOrEmpty(request.Address) ? user.Address : request.Address);
			user.FullName = request.FullName;
			user.Email = request.Email.ToLower();

			if (request.ThumbnailImage != null)
			{
				user.Avatar = await this.SaveFile(request.ThumbnailImage);
			}

			_context.Accounts.Update(user);
			return await _context.SaveChangesAsync();
		}

		private async Task<string> SaveFile(IFormFile file)
		{
			var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
			//var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
			await _storageService.SaveFileAsync(file.OpenReadStream(), originalFileName, "user");
			return "/" + USER_CONTENT_FOLDER_NAME + "/" + originalFileName;
		}

		public string ForgotPassword(Account user)
		{
			//string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}~!@#$%^&*()+";
			//Random rd = new Random();
			//StringBuilder sb = new StringBuilder();
			//for (int i = 0; i < 5; i++)
			//{
			//	sb.Append(pattern[rd.Next(0, pattern.Length)]);
			//}
			//string salt = sb.ToString();
			string date = DateTime.Now.ToString("dd/MM/yyyy");
			string time = DateTime.Now.ToString("HH-mm");

			var encodeEmai = _config["JWTSettings:SecretKey"] + ":" + user.Email + ":" + user.UserName + ":" + date + ":" + time;

			byte[] strBytes = Encoding.UTF8.GetBytes(encodeEmai);   //chuyển text thành bytes

			var tokenEmail = Convert.ToBase64String(strBytes);  //chuyển bytes thành string để máy hiểu

			string url = $"{_config["WebUrl"]}/User/ResetPassword?tokensEmail=" + tokenEmail;
			string titleEmail = "Khôi Phục Mật Khẩu";

			string contentEmail = "<h1>Làm theo hướng dẫn để khôi phục mật khẩu của bạn</h1>"
				+ $"<p>Để khôi phục mật khẩu. Bấm vào <a href='{url}'>đây</a>. Url này chỉ có hạn 5 phút.</p>";

			var result = _mailService.SendEmai(user.Email, titleEmail, contentEmail);
			if (result == true)
				return "Email khôi phục mật khẩu đã được gửi đến email đăng ký của bạn. URL chỉ có hạn dùng trong 5 phút";
			else
				return "Lỗi";
		}

		public async Task<string> ResetPassword(ResetPassRequest request)
		{
			var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Email == request.Email);
			string salt = Utilities.Helpper.Utilities.GetRandomKey();

			user.Password = (request.NewPass + salt.Trim()).ToMD5();
			user.KeySecurity = salt;

			_context.Accounts.Update(user);
			await _context.SaveChangesAsync();
			return "Đổi mật khẩu thành công";
		}

		public async Task<AccountVm> GetAccountById(int accountID)
		{
			var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == accountID && x.IsDelete == false);
			var result = new AccountVm()
			{
				AccountId = account.AccountId,
				FullName = account.FullName,
				UserName = account.UserName,
				Avatar = account.Avatar,
				PhoneNumber = account.PhoneNumber,
				Email = account.Email,
				Address = account.Address,
				CreateDate = account.CreateDate,
				NumberOfPost = account.NumberOfPost,
				Money = account.MoneyOfOver
			};
			return result;
		}

		public async Task<Account> GetAccountByPhone(string phone)
		{
			var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == phone && x.IsDelete == false);

			return account;
		}
	}
}