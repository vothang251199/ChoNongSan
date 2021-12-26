using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Extenstions;
using ChoNongSan.ViewModels.Requests.Admin;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCTV;
using ChoNongSan.ViewModels.Responses.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                UserName = request.UserName.ToLower(),
                Password = (request.Password + salt.Trim()).ToMD5(),
                //ConfirmPassword = (request.ConfirmPassword + salt.Trim()).ToMD5(),
                KeySecurity = salt,
                IsDelete = false,
                CreateDate = DateTime.Now,
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

        public async Task<List<ListCtvViewModel>> GetListCtv()
        {
            var lsCtv = await _context.Accounts.AsNoTracking().Where(x => x.RolesId == 2).ToListAsync();
            var viewmodel = lsCtv.Select(x => new ListCtvViewModel()
            {
                CtvID = x.AccountId,
                UserName = x.UserName,
            }).ToList();
            return viewmodel;
        }

        public async Task<int> UpdatePassCTV(UpdatePassCTVRequest request)
        {
            string salt = Utilities.Helpper.Utilities.GetRandomKey();

            var user = await _context.Accounts.FindAsync(request.AccountID);

            user.Password = (request.Password + salt.Trim()).ToMD5();
            //user.ConfirmPassword = (request.ConfirmPassword + salt.Trim()).ToMD5();

            _context.Accounts.Update(user);
            return await _context.SaveChangesAsync();
        }
    }
}