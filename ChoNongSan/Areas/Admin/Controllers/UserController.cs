using ChoNongSan.ApiService;
using ChoNongSan.ViewModels.Requests.Common.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return View();
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
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.Session.Remove("Token");
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