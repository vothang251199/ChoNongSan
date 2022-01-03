using ChoNongSan.Application.CTV;
using ChoNongSan.Application.KhachHang.Posts;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.CTV;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/ctv")]
    [ApiController]
    public class CtvsController : ControllerBase
    {
        private readonly ChoNongSanContext _context;
        private readonly ICtvService _ctvService;
        private readonly IPostService _postService;

        public CtvsController(ChoNongSanContext context, ICtvService ctvService, IPostService postService)
        {
            _postService = postService;
            _ctvService = ctvService;
            _context = context;
        }

        [HttpGet("danh-sach-bai-dang")]
        public async Task<IActionResult> GetListPost()
        {
            return Ok(await _postService.GetListPost());
        }

        [HttpGet("tim-kiem-bai-dang/{keyword}")]
        public async Task<IActionResult> GetAllPostBySearch(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var posts = await _postService.GetAllPostBySearch(keyword);
                return Ok(posts);
            }

            return BadRequest();
        }

        [HttpGet("phan-trang-bai-dang")]
        public async Task<IActionResult> GetAllBySearchPaging([FromQuery] GetSearchPostPagingRequest request)
        {
            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                var posts = await _postService.GetAllBySearchAndCatIdPaging(request);
                return Ok(posts);
            }

            return BadRequest();
        }

        [HttpPut("duyet-bai-dang")]
        public async Task<IActionResult> AcceptPost([FromForm] AcceptPostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var post = await _context.Posts.FindAsync(request.PostID);
            if (post == null) return BadRequest("Bài viết không tồn tại");

            var result = await _ctvService.AcceptPost(request);
            if (result == 0) return BadRequest();
            return Ok();
        }
    }
}