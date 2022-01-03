using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.Models;
using ChoNongSan.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostApi _postApi;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IPostApi postApi, IConfiguration config)
        {
            _logger = logger;
            _postApi = postApi;
            _config = config;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 2)
        {
            var request = new GetSearchPostPagingRequest()
            {
                KeyWord = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _postApi.GetPostPaging(request);
            ViewBag.Keyword = keyword;
            ViewBag.ApiUrl = _config["ApiUrl"];
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}