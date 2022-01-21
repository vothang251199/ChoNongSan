using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ViewModels.Requests.DonVi;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChoNongSan.AdminWeb.Controllers
{
    [Authorize]
    public class MgtWeightController : Controller
    {
        private readonly IWeightApi _weightApi;
        private readonly IConfiguration _config;

        public MgtWeightController(IWeightApi weightApi, IConfiguration config)
        {
            _weightApi = weightApi;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _weightApi.GetListWeight();
            ViewBag.Keyword = null;
            ViewBag.Link = "/MgtWeight";
            ViewBag.Obj = data;
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(CreateWeightRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var data = await _weightApi.Create(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return View();

            return RedirectToAction("Index", "MgtWeight");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int weightId)
        {
            var data = await _weightApi.GetWeightById(weightId);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);

            if (status.Contains("FAILED"))
            {
                TempData["ALertMessage"] = message;
                return RedirectToAction("Index", "MgtCtv");
            }

            var weight = (obj["data"]).ToObject<WeightVm>();
            var request = new UpdateWeightRequest()
            {
                WeightId = weight.WeightId,
                WeightName = weight.WeightName,
            };
            return View(request);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit(UpdateWeightRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var data = await _weightApi.Update(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return View();

            return RedirectToAction("Index", "MgtWeight");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int weightId)
        {
            var data = await _weightApi.GetWeightById(weightId);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);

            if (status.Contains("FAILED"))
            {
                TempData["ALertMessage"] = message;
                return View();
            }

            var weight = (obj["data"]).ToObject<WeightVm>();

            return View(weight);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(WeightVm weight)
        {
            var data = await _weightApi.Delete(weight.WeightId);
            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);
            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            if (status.Contains("FAILED"))
                return View();

            return RedirectToAction("Index", "MgtWeight");
        }
    }
}