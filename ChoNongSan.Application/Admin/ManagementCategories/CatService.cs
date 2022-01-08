using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhMuc;
using ChoNongSan.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<PageResult<CategoryVm>> GetCatsPaging(GetPagingCommonRequest request)
        {
            var lsCat = await _context.Categories.AsNoTracking().Where(x => x.IsDelete == false).ToListAsync();

            if (!String.IsNullOrEmpty(request.Keyword))
            {
                lsCat = lsCat.Where(x => x.CateName.ToLower().Contains(request.Keyword.ToLower())).ToList();
            }

            var totalRow = lsCat.Count();

            var data = lsCat.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CategoryVm()
                {
                    CategoryID = x.CategoryId,
                    CateName = x.CateName,
                }).ToList();

            var pageResult = new PageResult<CategoryVm>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
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
            cat.CateName = request.CatName.Trim();
            _context.Categories.Update(cat);
            return await _context.SaveChangesAsync();
        }
    }
}