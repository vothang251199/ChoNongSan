using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
using ChoNongSan.ViewModels.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
    public interface IPostApi
    {
        Task<PageResult<PostVmTongQuat>> GetPostPaging(GetPagingCommonRequest request);

        Task<PageResult<PostVmTongQuat>> GetAllByStatusPaging(GetPagingCommonRequest request);
        Task<string> GetAllLoveByAccountId(GetPagingCommonRequest request);

        Task<bool> CreatePost(CreatePostRequest request);
        Task<string> AddPostLove(LoveRequest request);
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

        public async Task<string> AddPostLove(LoveRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContnet = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);

            var response = await client.PostAsync($"/api/tin-dang/them-yeu-thich", httpContnet);

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<bool> CreatePost(CreatePostRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();

            if (request.ThumbnailImage != null)
            {
                foreach (var item in request.ThumbnailImage)
                {
                    byte[] data;
                    using (var br = new BinaryReader(item.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)item.OpenReadStream().Length);
                    }

                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "thumbnailImage", item.FileName);
                }
            }

            requestContent.Add(new StringContent(request.CategoryID.ToString()), "categoryID");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.ProductName) ? "" : request.ProductName), "productName");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Title) ? "" : request.Title), "title");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Description) ? "" : request.Description), "description");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.PhoneNumber) ? "" : request.PhoneNumber), "phoneNumber");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Address) ? "" : request.Address), "address");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");
            requestContent.Add(new StringContent(request.WeightId.ToString()), "weightId");
            requestContent.Add(new StringContent(request.WeightNumber.ToString()), "weightNumber");
            requestContent.Add(new StringContent(request.AccountID.ToString()), "accountID");
            requestContent.Add(new StringContent(request.IsDeliver.ToString()), "isDeliver");
            requestContent.Add(new StringContent(request.Expiry.ToString()), "expiry");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Quality) ? "" : request.Quality), "quality");

            var response = await client.PostAsync("/api/tin-dang/tao-moi", requestContent);

            var result = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<PageResult<PostVmTongQuat>> GetAllByStatusPaging(GetPagingCommonRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/tin-dang/tin-theo-trang-thai?ById={request.ById}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsPost = JsonConvert.DeserializeObject<PageResult<PostVmTongQuat>>(body);
            return lsPost;
        }

        public async Task<string> GetAllLoveByAccountId(GetPagingCommonRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/tin-dang/danh-sach-yeu-thich?ById={request.ById}" +
                $"&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            
            return body;
        }

        public async Task<PageResult<PostVmTongQuat>> GetPostPaging(GetPagingCommonRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/tin-dang/tin-dang-xem-chung?Keyword=" +
                $"{request.Keyword}&ById={request.ById}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var lsPost = JsonConvert.DeserializeObject<PageResult<PostVmTongQuat>>(body);
            return lsPost;
        }
    }
}