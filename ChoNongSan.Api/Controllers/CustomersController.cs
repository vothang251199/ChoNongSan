using ChoNongSan.Application.Common.Accounts;
using ChoNongSan.Application.KhachHang.PostImages;
using ChoNongSan.Application.KhachHang.Posts;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common.Accounts;
using ChoNongSan.ViewModels.Requests.KhachHang.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/khach-hang")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ChoNongSanContext _context;
        private readonly IPostService _postService;
        private readonly IAccountService _accountService;

        public CustomersController(ChoNongSanContext context, IPostService postService,
            IAccountService accountService)
        {
            _accountService = accountService;
            _postService = postService;
            _context = context;
        }

        [HttpPost("dang-ky")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == request.UserName.ToLower());
            var phoneExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            var emailExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Email == request.Email.ToLower());

            if (userExist != null) return BadRequest(new { message = "Tên tài khoản đã tồn tại", status = "FAILED" });
            if (phoneExist != null) return BadRequest(new { message = "Số điện thoại đã tồn tại", status = "FAILED" });
            if (emailExist != null) return BadRequest(new { message = "Email đã tồn tại.", status = "FAILED" });

            var result = await _accountService.Register(request);
            if (result == 1)
                return Ok(new { message = "Đăng ký tài khoản thành công", status = "OK" });
            else
                return BadRequest(new { message = "Đăng ký tài khoản thất bại", status = "FAILED" });
        }

        //Post
        [HttpGet("bai-dang-{postID}")]
        public async Task<IActionResult> GetPostById(int postID)
        {
            var id = await _context.Posts.FindAsync(postID);
            if (id == null) return BadRequest("Không tìm thấy bài viết");

            var post = await _postService.GetPostById(postID);
            return Ok(post);
        }

        [HttpGet("tat-ca-bai-dang")]
        public async Task<IActionResult> GetAllPost()
        {
            return Ok(await _postService.GetListPost());
        }

        [HttpGet("danh-sach-bai-dang-theo-danh-muc")]
        public async Task<IActionResult> GetAllPostByCategoryId([FromQuery] GetCatIDPostPagingRequest request)
        {
            var catExist = await _context.Categories.FindAsync(request.CategoryID);
            if (catExist == null) return BadRequest("Danh mục không tồn tại");
            var lsPostByCatId = await _postService.GetAllByCategoryID(request);
            return Ok(lsPostByCatId);
        }

        [HttpPost("tao-bai-dang")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _postService.CreatePost(request);
            if (result == 0) return BadRequest();
            return Ok("Tạo bài đăng thành công. Hãy chờ xét duyệt");
        }

        [HttpDelete("an-bai-dang/{PostID}")]
        public async Task<IActionResult> HiddenPost(int PostID)
        {
            var post = await _context.Posts.FindAsync(PostID);
            if (post == null) return BadRequest("Bài viết không tồn tài");

            var result = await _postService.HiddenPost(PostID);
            if (!result) return BadRequest();

            return Ok("Ẩn bài viết thành công");
        }

        //Images
        [HttpPost("bai-dang/{postID}/them-anh")]
        public async Task<IActionResult> CreateImagePost(int postID, [FromForm] PostImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageId = await _postService.AddImage(postID, request);
            if (imageId == 0) return BadRequest();

            var image = await _postService.GetImageById(imageId);
            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
        }

        [HttpPut("bai-dang/{postID}/cap-nhat-anh")]
        public async Task<IActionResult> UpdateImagePost([FromForm] PostImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageId = await _postService.UpdateImage(request.ImageID, request);
            if (imageId == 0) return BadRequest();

            var image = await _postService.GetImageById(imageId);
            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
        }

        [HttpDelete("{postID}/anh/{imageID}")]
        public async Task<IActionResult> RemoveImagePost(int imageID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _postService.RemoveImage(imageID);
            if (result == 0)
                return BadRequest();

            return Ok();
        }

        [HttpGet("bai-dang-{postID}/anh/{imageID}")]
        public async Task<IActionResult> GetImageById(int postID, int imageID)
        {
            var image = await _postService.GetImageById(imageID);
            if (image == null)
                return BadRequest("Không tìm thấy ảnh");
            return Ok(image);
        }
    }
}