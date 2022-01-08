using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoNongSan.Application.Common.Banners;
using ChoNongSan.Application.KhachHang.Posts;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.Banners;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/banner")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly ChoNongSanContext _context;

        public BannersController(IBannerService bannerService,
            ChoNongSanContext context)
        {
            _bannerService = bannerService;
            _context = context;
        }

        [HttpGet("tat-ca-banner")]
        public async Task<IActionResult> GetAllBanner()
        {
            return Ok(await _bannerService.GetAll());
        }

        [HttpPost("tao-bai-banner")]
        public async Task<IActionResult> CreateBanner([FromForm] AddBannerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bannerService.AddBanner(request);
            if (result == 0) return BadRequest("Thêm banner thất bại");
            return Ok("Thêm banner thành công");
        }

        [HttpDelete("xoa-banner/{BannerID}")]
        public async Task<IActionResult> DeleteBanner(int BannerID)
        {
            var banner = await _context.Banners.FindAsync(BannerID);
            if (banner == null) return BadRequest("Banner không tồn tài");
            if (banner.IsDelete == null) return BadRequest("Banner đã bị xóa");

            var result = await _bannerService.RemoveBanner(BannerID);
            if (!result) return BadRequest("Xóa banner thất bại");

            return Ok("Xóa banner thành công");
        }

        [HttpPut("cap-nhat-banner")]
        public async Task<IActionResult> UpdateBanner([FromForm] EditBannerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bannerExist = await _context.Banners.FindAsync(request.BannerId);
            if (bannerExist == null) return BadRequest("Banner không tồn tại");

            var result = await _bannerService.EditBanner(request);
            if (result == 0) return BadRequest("Cập nhật banner thất bại");
            return Ok("Cập nhật banner thành công");
        }

        [HttpGet("theo-trang-thai")]
        public async Task<IActionResult> GetAllBannerPagingByIsDelete([FromQuery] GetBannerPagingRequest request)
        {
            if (request.IsDelete != null)
                return Ok(await _bannerService.GetAllPaging(request));
            return BadRequest();
        }

        [HttpGet("banner/{BannerID}")]
        public async Task<IActionResult> GetBannerById(int BannerID)
        {
            var banner = await _bannerService.GetBannerById(BannerID);
            if (banner == null)
                return BadRequest("Không tìm thấy banner");
            return Ok(banner);
        }
    }
}