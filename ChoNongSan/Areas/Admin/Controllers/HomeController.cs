using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace ChoNongSan.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userName = User.Identity.Name;
            var avatar = User.Claims.Where(x => x.Type == ClaimTypes.Thumbprint)
                .Select(c => c.Value).SingleOrDefault();
            return View();
        }
    }
}