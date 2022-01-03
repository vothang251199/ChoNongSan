using ChoNongSan.ViewModels.Common;
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
    public interface IPostApi
    {
        Task<PageResult<PostVm>> GetPostPaging(GetSearchPostPagingRequest request);
    }

    public class PostApi : IPostApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public PostApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<PageResult<PostVm>> GetPostPaging(GetSearchPostPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/common/tat-ca-tin-dang?Keyword=" +
                $"{request.KeyWord}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsPost = JsonConvert.DeserializeObject<PageResult<PostVm>>(body);
            return lsPost;
        }
    }
}