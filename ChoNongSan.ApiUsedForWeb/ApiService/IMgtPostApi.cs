using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.KhachHang.Posts;
using ChoNongSan.ViewModels.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
    public interface IMgtPostApi
    {
        Task<PageResult<PostVm>> GetPostByStatusPaging(GetPostByStatusRequest request);
    }

    public class MgtPostApi : IMgtPostApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public MgtPostApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<PageResult<PostVm>> GetPostByStatusPaging(GetPostByStatusRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/khach-hang/tin-theo-trang-thai?Status=" +
                $"{request.Status}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsPost = JsonConvert.DeserializeObject<PageResult<PostVm>>(body);
            return lsPost;
        }
    }
}