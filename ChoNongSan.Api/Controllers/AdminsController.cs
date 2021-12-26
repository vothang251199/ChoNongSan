using ChoNongSan.Application.Admin.ManagementCategories;
using ChoNongSan.Application.Admin.ManagementCTVes;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCategory;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCTV;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers.Admins
{
    [Route("api/admin")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly ChoNongSanContext _context;
        private readonly IManagementCtvService _managementCtvService;
        private readonly ICatService _catService;

        public AdminsController(ChoNongSanContext context, IManagementCtvService managementCtvService,
            ICatService catService)
        {
            _catService = catService;
            _managementCtvService = managementCtvService;
            _context = context;
        }

        [HttpGet("tat-ca-danh-muc")]
        public async Task<IActionResult> GetListCategories()
        {
            return Ok(await _catService.GetListCat());
        }

        [HttpPost("them-moi-danh-muc")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var catExist = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CateName == request.CatName.ToLower());
            if (catExist != null) return BadRequest("Danh mục đã tồn tại");

            var result = await _catService.CreateCat(request);
            if (result == 0) return BadRequest("Thêm danh mục thất bại");

            return Ok("Thêm danh mục thành công");
        }

        [HttpPut("cap-nhat-danh-muc")]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var catExist = await _context.Categories.FindAsync(request.CatID);
            if (catExist == null) return BadRequest("Danh mục không tồn tại");

            var result = await _catService.UpdateCat(request);
            if (result == 0) return BadRequest("Cập nhật danh mục thất bại");
            return Ok("Cập nhật danh mục thành công");
        }

        [HttpDelete("xoa-danh-muc/{CategoryID}")]
        public async Task<IActionResult> DeleteCategory(int CategoryID)
        {
            var cat = await _context.Categories.FindAsync(CategoryID);
            if (cat == null) return BadRequest("Danh mục không tồn tại");

            var result = await _catService.DeleteCat(CategoryID);
            if (!result) return BadRequest("Xóa danh mục thất bại");
            return Ok("Xóa danh mục thành công");
        }

        [HttpGet("danh-sach-ctv")]
        public async Task<IActionResult> GetLAllCtvs()
        {
            var data = await _managementCtvService.GetListCtv();
            return Ok(data);
        }

        [HttpPost("them-ctv")]
        public async Task<IActionResult> CreateCTV([FromForm] CreateCTVRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == request.UserName.ToLower());
            if (userExist != null) return BadRequest("Tài khoản đã tồn tại");

            var result = await _managementCtvService.CreateCTV(request);
            if (result == 0) return BadRequest("Tạo Cộng Tác Viên không thành công");

            return Ok("Tạo Cộng Tác Viên thành công");
        }

        [HttpPut("doi-mat-khau-ctv")]
        public async Task<IActionResult> UpdateCTV([FromForm] UpdatePassCTVRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Accounts.FindAsync(request.AccountID);
            if (user == null) return BadRequest("Tài khoản không tồn tại");

            var result = await _managementCtvService.UpdatePassCTV(request);
            if (result == 0) return BadRequest("Thay đổi mật khẩu không thành công");

            return Ok("Đổi mật khẩu Cộng tác viên thành công");
        }

        [HttpDelete("xoa-ctv/{AccountID}")]
        public async Task<IActionResult> DeleteCTV(int AccountID)
        {
            var user = await _context.Accounts.FindAsync(AccountID);
            if (user == null) return BadRequest("Tài khoản không tồn tại");

            var result = await _managementCtvService.DeleteCTV(AccountID);
            if (result == 0) return BadRequest("Xóa Cộng tác viên không thành công");

            return Ok("Xóa Cộng tác viên thành công");
        }
    }
}