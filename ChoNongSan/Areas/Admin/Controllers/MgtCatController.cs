using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoNongSan.ApiService;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCategory;
using ChoNongSan.ViewModels.Responses.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChoNongSan.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class MgtCatController : Controller
    {
        private readonly ICategoryApi _categoryApi;
        private readonly IConfiguration _config;

        public MgtCatController(ICategoryApi categoryApi, IConfiguration config)
        {
            _categoryApi = categoryApi;
            _config = config;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 2)
        {
            var request = new GetCatsPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _categoryApi.GetCatPaging(request);
            ViewBag.Keyword = keyword;
            ViewBag.Link = "/Admin/MgtCat";
            ViewBag.Obj = data;
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCatRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var data = await _categoryApi.CreateCat(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return View();

            return RedirectToAction("Index", "MgtCat");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int categoryId)
        {
            var data = await _categoryApi.GetCatById(categoryId);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);

            if (status.Contains("FAILED"))
            {
                TempData["ALertMessage"] = message;
                return RedirectToAction("Index", "MgtCat");
            }

            CategoryVm cat = (obj["data"]).ToObject<CategoryVm>();
            var request = new UpdateCatRequest()
            {
                CatID = cat.CategoryID,
                CatName = cat.CateName,
            };
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCatRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var data = await _categoryApi.EditCat(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return View();

            return RedirectToAction("Index", "MgtCat");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int categoryId)
        {
            var data = await _categoryApi.GetCatById(categoryId);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);

            if (status.Contains("FAILED"))
            {
                TempData["ALertMessage"] = message;
                return RedirectToAction("Index", "MgtCat");
            }

            CategoryVm cat = (obj["data"]).ToObject<CategoryVm>();
            var request = new UpdateCatRequest()
            {
                CatID = cat.CategoryID,
                CatName = cat.CateName,
            };
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateCatRequest request)
        {
            var data = await _categoryApi.DeleteCat(request.CatID);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return RedirectToAction("Index", "MgtCat");

            return RedirectToAction("Index", "MgtCat");
        }
    }
}