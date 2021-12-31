using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCategory;
using ChoNongSan.ViewModels.Responses.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChoNongSan.ApiService
{
    public interface ICategoryApi
    {
        Task<PageResult<CategoryVm>> GetCatPaging(GetCatsPagingRequest request);

        Task<string> CreateCat(CreateCatRequest request);

        Task<string> EditCat(UpdateCatRequest request);

        Task<string> GetCatById(int catId);

        Task<string> DeleteCat(int catId);
    }

    public class CategoryApi : ICategoryApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public CategoryApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<PageResult<CategoryVm>> GetCatPaging(GetCatsPagingRequest request)
        {
            var apiurl = _config["AppSettings:ApiUrl"];

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(apiurl + $"/api/admin/danh-muc-phan-trang?Keyword=" +
                $"{request.Keyword}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsCat = JsonConvert.DeserializeObject<PageResult<CategoryVm>>(body);
            return lsCat;
        }

        public async Task<string> CreateCat(CreateCatRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["AppSettings:ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.CatName) ? "" : request.CatName), "catName");

            var response = await client.PostAsync("/api/admin/them-moi-danh-muc", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> EditCat(UpdateCatRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["AppSettings:ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.CatName) ? "" : request.CatName), "catName");

            var response = await client.PutAsync($"/api/admin/cap-nhat-danh-muc/{request.CatID}", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> GetCatById(int catId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["AppSettings:ApiUrl"]);

            var response = await client.GetAsync($"/api/admin/danh-muc/{catId}");

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> DeleteCat(int catId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["AppSettings:ApiUrl"]);

            var response = await client.DeleteAsync($"/api/admin/xoa-danh-muc/{catId}");

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }
    }
}