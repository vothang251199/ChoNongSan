using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCategory;
using ChoNongSan.ViewModels.Responses.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Admin.ManagementCategories
{
    public class CatService : ICatService
    {
        private readonly ChoNongSanContext _context;

        public CatService(ChoNongSanContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCat(CreateCatRequest request)
        {
            var cat = new Category()
            {
                CateName = request.CatName.ToLower(),
                IsDelete = false
            };
            _context.Categories.Add(cat);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCat(int CatID)
        {
            try
            {
                var cat = await _context.Categories.FindAsync(CatID);
                cat.IsDelete = true;
                _context.Categories.Update(cat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<CategoryVm> GetCatById(int CatID)
        {
            var cat = await _context.Categories.FindAsync(CatID);
            var result = new CategoryVm()
            {
                CategoryID = cat.CategoryId,
                CateName = cat.CateName,
            };
            return result;
        }

        public async Task<List<CategoryVm>> GetListCat()
        {
            var data = await _context.Categories.AsNoTracking().Where(x => x.IsDelete == false).ToListAsync();
            var lsCat = data.Select(x => new CategoryVm()
            {
                CategoryID = x.CategoryId,
                CateName = x.CateName,
            }).ToList();
            return lsCat;
        }

        public async Task<int> UpdateCat(UpdateCatRequest request)
        {
            var cat = await _context.Categories.FindAsync(request.CatID);
            cat.CateName = request.CatName.ToLower();
            _context.Categories.Update(cat);
            return await _context.SaveChangesAsync();
        }
    }
}