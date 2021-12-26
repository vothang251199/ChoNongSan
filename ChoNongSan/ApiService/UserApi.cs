using ChoNongSan.ViewModels.Requests.Common.Accounts;
using ChoNongSan.ViewModels.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiService
{
    public class UserApi : IUserApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public UserApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<string> Login(LoginRequest request)
        {
            var apiurl = _config["AppSettings:ApiUrl"];
            var json = JsonConvert.SerializeObject(request);
            var httpContnet = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("")
            var response = await client.PostAsync(apiurl + "/api/tai-khoan/dang-nhap", httpContnet);
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