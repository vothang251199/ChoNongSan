using ChoNongSan.ApiUsedForWeb.ApiService;
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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.AdminWeb.Controllers
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
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
                return View("Login");
            }

            var token = Convert.ToString(obj["data"]["token"]);
            var role = Convert.ToInt32(obj["data"]["account"]["rolesId"]);

            if (role != 1)
            {
                TempData["ALertMessage"] = "Bạn không có quyền truy cập vào trang này";
                return View("Login");
            }
            var userPrincipal = this.ValidateToken(token);
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = true, //lưu tài khoản để ko cần đăng nhập lại
            };

            HttpContext.Session.SetString("Token", token);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                authProperties);
            return RedirectToAction("Index", "MgtCat");
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

        [Authorize]
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            var a = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();
            ViewBag.Id = Convert.ToInt32(a);
            return View();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> DoiMatKhau(ChangePassRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var data = await _userApi.ChangePass(request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);

            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            return RedirectToAction("Index", "MgtCat");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CapNhatTaiKhoan()
        {
            var id = User.Claims.Where(x => x.Type == "Id").Select(c => c.Value).SingleOrDefault();

            var user = _userApi.GetUserById(Convert.ToInt32(id));
            var request = new UpdateAccountRequest()
            {
                AccountID = user.Result.AccountId,
                Address = user.Result.Address,
                Email = user.Result.Email,
                FullName = user.Result.FullName,
                PhoneNumber = user.Result.PhoneNumber,
                UserName = user.Result.UserName,
            };
            ViewBag.Avatar = _config["ApiUrl"] + user.Result.Avatar;

            ViewBag.Id = user.Result.AccountId;
            return View(request);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CapNhatTaiKhoan(UpdateAccountRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var data = await _userApi.Update(request.AccountID, request);
            var obj = (JObject)JsonConvert.DeserializeObject(data);

            var message = Convert.ToString(obj["message"]);
            TempData["ALertMessage"] = message;
            return RedirectToAction("Index", "MgtCat");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Login", "User");
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