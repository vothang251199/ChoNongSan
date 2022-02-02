using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace ChoNongSan.AdminWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userName = User.Identity.Name;
            //var avatar = User.Claims.Where(x => x.Type == ClaimTypes.Thumbprint)
            //    .Select(c => c.Value).SingleOrDefault();
            return RedirectToAction("Index", "MgtCat");
        }
    }
}