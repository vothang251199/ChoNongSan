using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCTV;
using ChoNongSan.ViewModels.Responses.Admin;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
    public interface ICtvApi
    {
        Task<PageResult<CtvVm>> GetCtvPaging(GetCtvPagingRequest request);

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

        public async Task<PageResult<CtvVm>> GetCtvPaging(GetCtvPagingRequest request)
        {
            var apiurl = _config["ApiUrl"];

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(apiurl + $"/api/admin/ctv-phan-trang?Keyword=" +
                $"{request.Keyword}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsCat = JsonConvert.DeserializeObject<PageResult<CtvVm>>(body);
            return lsCat;
        }

        public async Task<string> CreateCtv(CreateCTVRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.UserName) ? "" : request.UserName), "userName");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Password) ? "" : request.Password), "password");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.ConfirmPassword) ? "" : request.ConfirmPassword), "confirmPassword");

            var response = await client.PostAsync("/api/admin/them-ctv", requestContent);

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

            var response = await client.PutAsync($"/api/admin/doi-mat-khau-ctv/{request.AccountID}", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }
    }
}