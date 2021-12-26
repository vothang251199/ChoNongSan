using ChoNongSan.Application.Common.Files;
using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Exceptions;
using ChoNongSan.Utilities.Extenstions;
using ChoNongSan.ViewModels.Requests.Common.Accounts;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly ChoNongSanContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public AccountService(ChoNongSanContext context, IStorageService storageService,
            IConfiguration config)
        {
            _storageService = storageService;
            _context = context;
            _config = config;
        }

        public async Task<int> ChangePassword(ChangePassRequest request)
        {
            string salt = Utilities.Helpper.Utilities.GetRandomKey();

            var user = await _context.Accounts.FindAsync(request.AccountID);

            user.Password = (request.NewPass + salt.Trim()).ToMD5();
            user.KeySecurity = salt;
            //user.ConfirmPassword = (request.ConfirmPassword + salt.Trim()).ToMD5();

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

        public async Task<List<Account>> GetAll()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<LoginViewModel> Login(LoginRequest request)
        {
            var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserName.Contains(request.LoginName.ToLower())
                || x.PhoneNumber.Contains(request.LoginName) || x.Email.Contains(request.LoginName.ToLower()));
            var role = user.RolesId;
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.LoginName.ToLower()),
                new Claim(ClaimTypes.Role, Convert.ToString(role)),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);
            var result = new LoginViewModel()
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
                //ConfirmPassword = (request.ConfirmPassword + salt.Trim()).ToMD5(),
                KeySecurity = salt,
                IsDelete = false,
                CreateDate = DateTime.Now,
                RolesId = 3,
            };

            _context.Accounts.Add(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(UpdateAccountRequest request)
        {
            var user = await _context.Accounts.FindAsync(request.AccountID);

            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
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
    }
}