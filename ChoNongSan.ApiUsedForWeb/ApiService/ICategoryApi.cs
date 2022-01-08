using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhMuc;
using ChoNongSan.ViewModels.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
    public interface ICategoryApi
    {
        Task<PageResult<CategoryVm>> GetCatPaging(GetPagingCommonRequest request);

        Task<List<CategoryVm>> GetListCat();

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

        public async Task<PageResult<CategoryVm>> GetCatPaging(GetPagingCommonRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/danh-muc/danh-muc-phan-trang?Keyword=" +
                $"{request.Keyword}&ById={request.ById}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsCat = JsonConvert.DeserializeObject<PageResult<CategoryVm>>(body);
            return lsCat;
        }

        public async Task<string> CreateCat(CreateCatRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.CatName) ? "" : request.CatName), "catName");

            var response = await client.PostAsync("/api/danh-muc/them-moi-danh-muc", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> EditCat(UpdateCatRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.CatName) ? "" : request.CatName), "catName");

            var response = await client.PutAsync($"/api/danh-muc/cap-nhat-danh-muc/{request.CatID}", requestContent);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> GetCatById(int catId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);

            var response = await client.GetAsync($"/api/danh-muc/{catId}");

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<string> DeleteCat(int catId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);

            var response = await client.DeleteAsync($"/api/danh-muc/xoa-danh-muc/{catId}");

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<List<CategoryVm>> GetListCat()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);

            var response = await client.GetAsync($"/api/danh-muc/tat-ca-danh-muc");

            var body = await response.Content.ReadAsStringAsync();
            var lsCat = JsonConvert.DeserializeObject<List<CategoryVm>>(body);
            return lsCat;
        }
    }
}