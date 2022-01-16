using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.Application.Common.Files;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
    [Authorize]
    public class QuanLyTinDang : Controller
    {
        private readonly IConfiguration _config;
        private readonly IPostApi _postApi;
        private readonly IStorageService _storageService;
        private readonly ICategoryApi _categoryApi;
        private readonly IWeightApi _weightApi;
        private readonly IUserApi _userApi;

        public QuanLyTinDang(IConfiguration config, IPostApi postApi, IStorageService storageService,
            ICategoryApi categoryApi, IWeightApi weightApi, IUserApi userApi)
        {
            _userApi = userApi;
            _weightApi = weightApi;
            _categoryApi = categoryApi;
            _storageService = storageService;
            _config = config;
            _postApi = postApi;
        }

        [HttpGet]
        public IActionResult Index(PostTabVm vm, int pageIndex = 1, int pageSize = 5)
        {
            if (vm == null)
            {
                vm = new PostTabVm
                {
                    ActiveTab = Tab.HienThi,
                };
            }
            vm.pageIndex = pageIndex;
            vm.pageSize = pageSize;

            return View(vm);
        }

        public IActionResult SwitchTabs(string tabname)
        {
            var vm = new PostTabVm();
            switch (tabname)
            {
                case "HienThi":
                    vm.ActiveTab = Tab.HienThi;
                    break;

                case "DoiDuyet":
                    vm.ActiveTab = Tab.DoiDuyet;
                    break;

                case "TuChoi":
                    vm.ActiveTab = Tab.TuChoi;
                    break;

                case "DaAn":
                    vm.ActiveTab = Tab.DaAn;
                    break;

                default:
                    vm.ActiveTab = Tab.HienThi;
                    break;
            }
            return RedirectToAction(nameof(QuanLyTinDang.Index), vm);
        }

        [HttpGet]
        public async Task<IActionResult> TaoMoi()
        {
            ViewBag.CategoryList = await _categoryApi.GetListCat();
            ViewBag.WeightList = await _weightApi.GetListWeight();

            var userId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
            var user = await _userApi.GetUserById(Convert.ToInt32(userId));
            ViewBag.Phone = user.PhoneNumber;
            ViewBag.Address = user.Address;

            return View();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> TaoMoi(CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = await _categoryApi.GetListCat();
                ViewBag.WeightList = await _weightApi.GetListWeight();
                var userId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
                var user = await _userApi.GetUserById(Convert.ToInt32(userId));
                ViewBag.Phone = user.PhoneNumber;
                ViewBag.Address = user.Address;
                return View(request);
            }

            var result = await _postApi.CreatePost(request);
            if (!result)
            {
                TempData["ALertMessage"] = "Đăng tin không thành công";
                return View();
            }
            TempData["ALertMessage"] = "Đăng tin thành công, chờ hệ thống xét duyệt";
            var vm = new PostTabVm
            {
                ActiveTab = Tab.DoiDuyet
            };

            return RedirectToAction(nameof(QuanLyTinDang.Index), vm);
        }

        [HttpGet]
        public async Task<IActionResult> YeuThich(int pageIndex = 1, int pageSize = 4)
        {
            var userId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
            GetPagingCommonRequest request = new GetPagingCommonRequest();

            request.ById = Convert.ToInt32(userId);
            request.PageIndex = pageIndex;
            request.PageSize = pageSize;

            var result = await _postApi.GetAllLoveByAccountId(request);
            var obj = (JObject)JsonConvert.DeserializeObject(result);
            var status = Convert.ToString(obj["status"]);
            var model = obj["data"].ToObject<PageResult<PostVmTongQuat>>();
            foreach (var i in model.Items)
            {
                i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
            }
            return View(model);
        }

        public async Task<IActionResult> AddlovePost([FromBody] string postId)
        {
            var accountId = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
            var request = new LoveRequest()
            {
                accountId = Convert.ToInt32(accountId),
                postId = Convert.ToInt32(postId)
            };
            var data = await _postApi.AddPostLove(request);

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int postId)
        {
            var model = await _postApi.GetDetail(postId);
            return View(model);
        }
    }
}