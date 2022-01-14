using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.Controllers.Components;
using ChoNongSan.ViewModels.Requests.TaiKhoan;
using ChoNongSan.ViewModels.Requests.TaiKhoan.KhachHang;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserApi _userApi;
        private readonly IConfiguration _config;

        public UserController(IUserApi userApi, IConfiguration config)
        {
            _userApi = userApi;
            _config = config;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Title = "Đăng ký";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var data = await _userApi.Register(request);

            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);

            var message = Convert.ToString(obj["message"]);
            if (status.Contains("FAILED"))
            {
                TempData["ALertMessage"] = message;
                return View();
            }
            TempData["ALertMessage"] = message;
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.Title = "Đăng nhập";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var data = await _userApi.Login(request);

            var obj = (JObject)JsonConvert.DeserializeObject(data);
            var status = Convert.ToString(obj["status"]);

            if (status.Contains("FAILED"))
            {
                var message = Convert.ToString(obj["message"]);
                TempData["ALertMessage"] = message;
                return View();
            }

            var token = Convert.ToString(obj["data"]["token"]);
            var role = Convert.ToInt32(obj["data"]["account"]["rolesId"]);

            if (role != 3)
            {
                TempData["ALertMessage"] = "Bạn không có quyền truy cập vào trang này";
                return View();
            }
            var userPrincipal = this.ValidateToken(token);
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                IsPersistent = true, //lưu tài khoản để ko cần đăng nhập lại
            };

            HttpContext.Session.SetString("Token", token);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                authProperties);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var data = await _userApi.ForgotPassword(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);

            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            return RedirectToAction("ForgotPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword(string tokensEmail)
        {
            try
            {
                var bytes = Convert.FromBase64String(tokensEmail);
                string[] decoded = Encoding.UTF8.GetString(bytes).Split(":");
                var email = decoded[1];

                var user = new ResetPassRequest()
                {
                    Email = email,
                };

                return View(user);
            }
            catch (Exception)
            {
                TempData["ALertMessage"] = "Email không chính xác";
                return RedirectToAction("ForgotPassword", "User");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var data = await _userApi.ResetPassword(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);

            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Login", "User");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile(ProfileTabVm vm)
        {
            var accountID = User.Claims.Where(x => x.Type == "Id")
                   .Select(c => c.Value).SingleOrDefault();
            vm.accountId = Convert.ToInt32(accountID);
            if (vm == null)
            {
                vm.ActiveTab = TabProfile.ThongTin;
            }

            ViewBag.HiddenLayOut = 1;

            return View(vm);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CapNhatTaiKhoan(UpdateAccountRequest request)
        {
            var accountID = User.Claims.Where(x => x.Type == "Id")
               .Select(c => c.Value).SingleOrDefault();
            if (!ModelState.IsValid)
            {
                var vm = new ProfileTabVm()
                {
                    ActiveTab = TabProfile.CapNhat,
                    status = "yes",
                    accountId = Convert.ToInt32(accountID),
                    Request = request,
                };

                return View("Profile", vm);
            }
            else
            {
                var vm = new ProfileTabVm()
                {
                    ActiveTab = TabProfile.ThongTin,
                    status = "no",
                    accountId = Convert.ToInt32(accountID),
                    Request = request,
                };
                await _userApi.Update(vm.accountId, vm.Request);
                return View("Profile", vm);
            }
        }

        public IActionResult SwitchTabs(string tabname)
        {
            var vm = new ProfileTabVm();
            switch (tabname)
            {
                case "TaiKhoan":
                    vm.ActiveTab = TabProfile.ThongTin;
                    break;

                case "YeuThich":
                    vm.ActiveTab = TabProfile.YeuThich;
                    break;

                case "CapNhat":
                    vm.ActiveTab = TabProfile.CapNhat;
                    break;

                case "DoiMK":
                    vm.ActiveTab = TabProfile.DoiMK;
                    break;

                default:
                    vm.ActiveTab = TabProfile.ThongTin;
                    break;
            }
            return RedirectToAction(nameof(UserController.Profile), vm);
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;
            SecurityToken validateToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _config["Tokens:Issuer"];
            validationParameters.ValidIssuer = _config["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validateToken);
            return principal;
        }
    }
}