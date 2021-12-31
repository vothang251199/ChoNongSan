using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoNongSan.ApiService;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCTV;
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
    public class MgtCTVController : Controller
    {
        private readonly ICtvApi _ctvApi;
        private readonly IConfiguration _config;

        public MgtCTVController(ICtvApi ctvApi, IConfiguration config)
        {
            _ctvApi = ctvApi;
            _config = config;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 2)
        {
            var request = new GetCtvPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _ctvApi.GetCtvPaging(request);
            ViewBag.Keyword = keyword;
            ViewBag.Link = "/Admin/MgtCtv";
            ViewBag.Obj = data;
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCTVRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var data = await _ctvApi.CreateCtv(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return View();

            return RedirectToAction("Index", "MgtCtv");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int ctvId)
        {
            var data = await _ctvApi.GetCtvById(ctvId);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);

            if (status.Contains("FAILED"))
            {
                TempData["ALertMessage"] = message;
                return RedirectToAction("Index", "MgtCtv");
            }

            CtvVm ctv = (obj["data"]).ToObject<CtvVm>();
            var request = new UpdatePassCTVRequest()
            {
                AccountID = ctv.AccountId,
                UserName = ctv.UserName,
                Password = "",
                ConfirmPassword = "",
            };
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePassCTVRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var data = await _ctvApi.EditCtv(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return View();

            return RedirectToAction("Index", "MgtCtv");
        }
    }
}