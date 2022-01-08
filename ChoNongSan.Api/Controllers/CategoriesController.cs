using System.Threading.Tasks;
using ChoNongSan.Application.Admin.ManagementCategories;
using ChoNongSan.Application.Admin.ManagementCTVes;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhMuc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/danh-muc")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ChoNongSanContext _context;
        private readonly IManagementCtvService _mgtCtvService;
        private readonly ICatService _catService;

        public CategoriesController(ChoNongSanContext context, IManagementCtvService managementCtvService,
            ICatService catService)
        {
            _catService = catService;
            _mgtCtvService = managementCtvService;
            _context = context;
        }

        [HttpGet("danh-muc/{CategoryID}")]
        public async Task<IActionResult> GetCatById(int CategoryID)
        {
            var cat = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == CategoryID && x.IsDelete == false);
            if (cat == null) return BadRequest(new { message = "Không tìm thấy danh mục", status = "FAILED" });

            var data = await _catService.GetCatById(CategoryID);
            return Ok(new { data = data, status = "OK" });
        }

        [HttpGet("tat-ca-danh-muc")]
        public async Task<IActionResult> GetListCategories()
        {
            return Ok(await _catService.GetListCat());
        }

        //https:localhost:5001/api/admin/danh-muc-phan-trang?pageIndex=0&pageSize=5
        [HttpGet("danh-muc-phan-trang")]
        public async Task<IActionResult> GetCatsPaging([FromQuery] GetPagingCommonRequest request)
        {
            return Ok(await _catService.GetCatsPaging(request));
        }

        [HttpPost("them-moi-danh-muc")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var catExist = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CateName.ToLower().Contains(request.CatName.ToLower()));
            if (catExist != null)
                return BadRequest(new { message = "Danh mục đã tồn tại", status = "FAILED" });

            var result = await _catService.CreateCat(request);
            if (result == 0)
                return BadRequest(new { message = "Thêm danh mục thất bại", status = "FAILED" });
            return Ok(new { message = "Thêm danh mục thành công", status = "OK" });
        }

        [HttpPut("cap-nhat-danh-muc/{CategoryID}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int CategoryID, [FromForm] UpdateCatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.CatID = CategoryID;
            var catExist = await _context.Categories.FindAsync(request.CatID);
            if (catExist == null)
                return BadRequest(new { message = "Danh mục không tồn tại", status = "FAILED" });

            var result = await _catService.UpdateCat(request);
            if (result == 0) return BadRequest(new { message = "Cập nhật danh mục thất bại", status = "FAILED" });
            return Ok(new { message = "Cập nhật danh mục thành công", status = "OK" });
        }

        [HttpDelete("xoa-danh-muc/{CategoryID}")]
        public async Task<IActionResult> DeleteCategory(int CategoryID)
        {
            var cat = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == CategoryID && x.IsDelete == false);
            if (cat == null) return BadRequest(new { message = "Không tìm thấy danh mục", status = "FAILED" });

            var result = await _catService.DeleteCat(CategoryID);
            if (!result)
                return BadRequest(new { message = "Xóa danh mục thất bại", status = "FAILED" });
            return Ok(new { message = "Xóa danh mục thành công", status = "OK" });
        }
    }
}