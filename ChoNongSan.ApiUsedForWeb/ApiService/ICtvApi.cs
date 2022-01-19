using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TaiKhoan.Ctv;
using ChoNongSan.ViewModels.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
    public interface ICtvApi
    {
        Task<PageResult<AccountVm>> GetCtvPaging(GetPagingCommonRequest request);

        Task<string> CreateCtv(CreateCTVRequest request);

        Task<string> GetCtvById(int ctvId);

        Task<string> EditCtv(UpdatePassCTVRequest request);
    }

    public class CtvApi : ICtvApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public CtvApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<string> GetCtvById(int ctvId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);

            var response = await client.GetAsync($"/api/tai-khoan/{ctvId}");

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<PageResult<AccountVm>> GetCtvPaging(GetPagingCommonRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/tai-khoan/all-account?Keyword={request.Keyword}&ById={request.ById}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsCat = JsonConvert.DeserializeObject<PageResult<AccountVm>>(body);
            return lsCat;
        }

        public async Task<string> CreateCtv(CreateCTVRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.UserName) ? "" : request.UserName), "userName");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Password) ? "" : request.Password), "password");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.ConfirmPassword) ? "" : request.ConfirmPassword), "confirmPassword");

            var response = await client.PostAsync("/api/tai-khoan/them-ctv", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> EditCtv(UpdatePassCTVRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Password) ? "" : request.Password), "password");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.ConfirmPassword) ? "" : request.ConfirmPassword), "confirmPassword");

            var response = await client.PutAsync($"/api/tai-khoan/doi-mat-khau-ctv/{request.AccountID}", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }
    }
}