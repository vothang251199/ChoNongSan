using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.Controllers.Components;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.KhachHang.Posts;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
    [Authorize]
    public class QuanLyTinDangController : Controller
    {
        private readonly IMgtPostApi _mgtPostApi;
        private readonly IConfiguration _config;

        public QuanLyTinDangController(IMgtPostApi mgtPostApi, IConfiguration config)
        {
            _mgtPostApi = mgtPostApi;
            _config = config;
        }

        // GET: PostController
        public async Task<ActionResult> Index(string statusPost, int pageIndex = 1, int pageSize = 2)
        {
            if (String.IsNullOrEmpty(statusPost)) statusPost = "2";
            var request = new GetPostByStatusRequest()
            {
                Status = Convert.ToInt32(statusPost),
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _mgtPostApi.GetPostByStatusPaging(request);

            ViewBag.ApiUrl = _config["ApiUrl"];
            return View(data);
        }

        // GET: PostController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PostController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PostController/Create
        [HttpPost]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PostController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PostController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PostController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PostController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}