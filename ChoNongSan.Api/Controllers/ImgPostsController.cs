using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoNongSan.Application.Common.Accounts;
using ChoNongSan.Application.KhachHang.PostImages;
using ChoNongSan.Application.KhachHang.Posts;
using ChoNongSan.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/anh")]
    [ApiController]
    public class ImgPostsController : ControllerBase
    {
        private readonly ChoNongSanContext _context;
        private readonly IPostService _postService;
        private readonly IAccountService _accountService;

        public ImgPostsController(ChoNongSanContext context, IPostService postService,
            IAccountService accountService)
        {
            _accountService = accountService;
            _postService = postService;
            _context = context;
        }

        //Api cho ảnh của tin đăng
        [HttpPost("tin-dang/{postID}/them-anh")]
        [Consumes("multipart/form-data")]
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

        [HttpPut("tin-dang/{postID}/cap-nhat-anh")]
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

        [HttpDelete("tin-dang/{postID}/anh/{imageID}")]
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

        [HttpGet("tin-dang/{postID}/anh/{imageID}")]
        public async Task<IActionResult> GetImageById(int postID, int imageID)
        {
            var image = await _postService.GetImageById(imageID);
            if (image == null)
                return BadRequest("Không tìm thấy ảnh");
            return Ok(image);
        }
    }
}