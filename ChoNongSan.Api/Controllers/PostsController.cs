using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoNongSan.Application.Common.Accounts;
using ChoNongSan.Application.CTV;
using ChoNongSan.Application.KhachHang.Posts;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.CTV;
using ChoNongSan.ViewModels.Requests.TinDang;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/tin-dang")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ChoNongSanContext _context;
        private readonly IPostService _postService;
        private readonly ICtvService _ctvService;

        public PostsController(ChoNongSanContext context, IPostService postService, ICtvService ctvService)
        {
            _postService = postService;
            _ctvService = ctvService;
            _context = context;
        }

        [HttpGet("{postID}")]
        public async Task<IActionResult> GetPostById(int postID)
        {
            var id = await _context.Posts.FindAsync(postID);
            if (id == null) return BadRequest(new { message = "Không tìm thấy bài viết", status="FAILED"});

            var post = await _postService.GetPostById(postID);
            return Ok(new {data= post, status = "OK" });
        }

        [HttpGet("danh-sach-yeu-thich")]
        public async Task<IActionResult> GetAllPostLoveByAccountId([FromQuery]  GetPagingCommonRequest request)
        {
            if (request.ById == 0 || request.ById == null)
                return BadRequest(new { message = "tài khoản không tồn tại", status = "FAILED" });
            return Ok(new { data = await _postService.GetAllLoveByAccountId(request), status="OK"});
        }

        [HttpGet("tat-ca-tin-dang-cho-app")]
        public async Task<IActionResult> GetListPostForApp()
        {
            return Ok(await _postService.GetListPostForApp());
        }

        [HttpGet("tin-dang-xem-chung")]
        public async Task<IActionResult> GetAllPostsViewHomePaging([FromQuery] GetPagingCommonRequest request)
        {
            return Ok(await _postService.GetAllPostsViewHome(request));
        }

        [HttpGet("tin-theo-trang-thai")]
        public async Task<IActionResult> GetAllPostByStatusPaging([FromQuery] GetPagingCommonRequest request)
        {
            var lsPost = await _postService.GetAllByStatusPaging(request);
            return Ok(lsPost);
        }

        [HttpPost("tao-moi")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _postService.CreatePost(request);
            if (result == 0) return BadRequest(new { message="Tạo tinđăng thất bại", status="FAILED"});
            return Ok(new { message = "Tạo bài đăng thành công. Hãy chờ xét duyệt", status = "OK" });
        }

        [HttpDelete("an-tin/{PostID}")]
        public async Task<IActionResult> HiddenPost(int PostID)
        {
            var post = await _context.Posts.FindAsync(PostID);
            if (post == null) return BadRequest("Bài viết không tồn tài");

            var result = await _postService.HiddenPost(PostID);
            if (!result) return BadRequest();

            return Ok("Ẩn bài viết thành công");
        }

        //Area Ctv
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

        [HttpPost("them-yeu-thich")]
        public async Task<IActionResult> AddPostLove([FromBody] LoveRequest request)
        {
            var user = await _context.Accounts.FindAsync(request.accountId);
            var post = await _context.Posts.FindAsync(request.postId);

            var lsExxist = _context.Loves.AsNoTracking().Where(x => x.AccountId == request.accountId && x.PostId == request.postId).ToList();
            if(lsExxist.Count != 0)
            {
                return Ok(new { message = "Tin đã có trong danh sách yêu thích của bạn", status = "OK" });
            }
            if (user == null)
                return BadRequest(new { message = "Tài khoản không tồn tại", status = "FAILED" });
            if (post == null)
                return BadRequest(new { message = "Không tìm thấy tin đăng", status = "FAILED" });

            var result = await _postService.AddLovePost(request);
            return Ok(new { message = result, status = "OK" });
        }
    }
}