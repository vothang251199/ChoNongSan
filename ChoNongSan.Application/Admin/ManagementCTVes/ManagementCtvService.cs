using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Extenstions;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TaiKhoan.Ctv;
using ChoNongSan.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Admin.ManagementCTVes
{
    public class ManagementCtvService : IManagementCtvService
    {
        private readonly ChoNongSanContext _context;

        public ManagementCtvService(ChoNongSanContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCTV(CreateCTVRequest request)
        {
            string salt = Utilities.Helpper.Utilities.GetRandomKey();

            var user = new Account()
            {
                UserName = request.UserName,
                FullName = request.UserName,
                Password = (request.Password + salt.Trim()).ToMD5(),
                KeySecurity = salt,
                IsDelete = false,
                CreateDate = DateTime.Now,
                Avatar = "/user-content/avatar-null.png",
                RolesId = 2,
            };

            _context.Accounts.Add(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteCTV(int AccountID)
        {
            var user = await _context.Accounts.FindAsync(AccountID);

            user.IsDelete = true;
            _context.Accounts.Update(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<PageResult<CtvVm>> GetCtvPaging(GetPagingCommonRequest request)
        {
            var lsCtv = await _context.Accounts.AsNoTracking().Where(x => x.RolesId == 2 && x.IsDelete == false).ToListAsync();

            if (!String.IsNullOrEmpty(request.Keyword))
            {
                lsCtv = lsCtv.Where(x => x.UserName.ToLower().Contains(request.Keyword.ToLower())).ToList();
            }

            var totalRow = lsCtv.Count();

            var data = lsCtv.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CtvVm()
                {
                    AccountId = x.AccountId,
                    UserName = x.UserName,
                    Phone = x.PhoneNumber,
                    Email = x.Email,
                }).ToList();

            var pageResult = new PageResult<CtvVm>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
        }

        public async Task<int> UpdatePassCTV(UpdatePassCTVRequest request)
        {
            string salt = Utilities.Helpper.Utilities.GetRandomKey();

            var user = await _context.Accounts.FindAsync(request.AccountID);

            user.Password = (request.Password + salt.Trim()).ToMD5();
            user.KeySecurity = salt;

            _context.Accounts.Update(user);
            return await _context.SaveChangesAsync();
        }
    }
}