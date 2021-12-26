using ChoNongSan.Application.Common.Files;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Common.Banners;
using ChoNongSan.ViewModels.Requests.Common.Banners;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common.Banners
{
    public class BannerService : IBannerService
    {
        private readonly ChoNongSanContext _context;
        private readonly IStorageService _storageService;
        private const string BANNER_CONTENT_FOLDER_NAME = "banner-content";

        public BannerService(ChoNongSanContext context, IStorageService storageService)
        {
            _storageService = storageService;
            _context = context;
        }

        public async Task<int> AddBanner(AddBannerRequest request)
        {
            var banner = new Banner()
            {
                CreateTime = DateTime.Now,
                Topic = request.Topic,
                IsDelete = false,
            };
            if (request.ThumbnailImage != null)
            {
                banner.ImagePath = await this.SaveFile(request.ThumbnailImage);
            }
            _context.Banners.Add(banner);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> EditBanner(EditBannerRequest request)
        {
            var banner = await _context.Banners.FindAsync(request.BannerId);
            banner.CreateTime = DateTime.Now;
            banner.Topic = request.Topic;
            banner.IsDelete = request.IsDelete;
            if (request.ThumbnailImage != null)
            {
                banner.ImagePath = await this.SaveFile(request.ThumbnailImage);
            }

            _context.Banners.Update(banner);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Banner>> GetAll()
        {
            var lsBanner = await _context.Banners.AsNoTracking().Where(x => x.IsDelete == false).ToListAsync();
            return lsBanner;
        }

        public async Task<PageResult<Banner>> GetAllPaging(GetBannerPagingRequest request)
        {
            List<Banner> lsBanner;

            if (request.IsDelete == true)
            {
                lsBanner = await _context.Banners.AsNoTracking().Where(x => x.IsDelete == request.IsDelete).ToListAsync();
            }
            else
            {
                lsBanner = await _context.Banners.AsNoTracking().Where(x => x.IsDelete == request.IsDelete).ToListAsync();
            }

            var totalRow = lsBanner.Count();

            var data = lsBanner.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).ToList();

            var pageResult = new PageResult<Banner>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
        }

        public async Task<Banner> GetBannerById(int BannerID)
        {
            var banner = await _context.Banners.FindAsync(BannerID);
            return banner;
        }

        public async Task<bool> RemoveBanner(int BannerID)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(BannerID);
                banner.IsDelete = true;
                _context.Banners.Update(banner);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), originalFileName, "banner");
            return "/" + BANNER_CONTENT_FOLDER_NAME + "/" + originalFileName;
        }
    }
}