using ChoNongSan.ViewModels.Requests.Common.Accounts;
using ChoNongSan.ViewModels.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
    public interface IUserApi
    {
        Task<string> Register(RegisterRequest request);

        Task<string> Login(LoginRequest request);

        Task<string> ForgotPassword(ForgetPasswordRequest request);

        Task<string> ResetPassword(ResetPassRequest request);
    }

    public class UserApi : IUserApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public UserApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<string> Register(RegisterRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.FullName) ? "" : request.FullName), "fullName");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.UserName) ? "" : request.UserName), "userName");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Email) ? "" : request.Email), "email");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.PhoneNumber) ? "" : request.PhoneNumber), "phoneNumber");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Password) ? "" : request.Password), "password");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.ConfirmPassword) ? "" : request.ConfirmPassword), "confirmPassword");

            var response = await client.PostAsync("/api/khach-hang/dang-ky", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> Login(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContnet = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.PostAsync("/api/tai-khoan/dang-nhap", httpContnet);

            var data = await response.Content.ReadAsStringAsync();
            return data;

            //var failed = new { message = "Gọi api thất bại", status = "FAILED" };
            //var rs = JsonConvert.SerializeObject(failed);
            //return (rs);
        }

        public async Task<string> ResetPassword(ResetPassRequest request)
        {
            var apiurl = _config["ApiUrl"];
            var json = JsonConvert.SerializeObject(request);
            var httpContnet = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("")
            var response = await client.PutAsync(apiurl + $"/api/tai-khoan/khoi-phuc-mat-khau", httpContnet);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return data;
            }
            var failed = new { message = "Gọi api thất bại", status = "FAILED" };
            var rs = JsonConvert.SerializeObject(failed);
            return (rs);
        }

        public async Task<string> ForgotPassword(ForgetPasswordRequest request)
        {
            var apiurl = _config["ApiUrl"];
            var json = JsonConvert.SerializeObject(request);
            var httpContnet = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("")
            var response = await client.PostAsync(apiurl + "/api/tai-khoan/quen-mat-khau", httpContnet);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return data;
            }
            var failed = new { message = "Gọi api thất bại", status = "FAILED" };
            var rs = JsonConvert.SerializeObject(failed);
            return (rs);
        }
    }
}