using ChoNongSan.ViewModels.Requests.DonVi;
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
    public interface IWeightApi
    {
        Task<List<WeightVm>> GetListWeight();

        Task<string> Create(CreateWeightRequest request);

        Task<string> Update(UpdateWeightRequest request);

        Task<string> Delete(int WeightId);

        Task<string> GetWeightById(int weightId);
    }

    public class WeightApi : IWeightApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public WeightApi(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<WeightVm>> GetListWeight()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var response = await client.GetAsync($"/api/don-vi/tat-ca");
            var body = await response.Content.ReadAsStringAsync();
            var lsWeight = JsonConvert.DeserializeObject<List<WeightVm>>(body);
            return lsWeight;
        }

        public async Task<string> Create(CreateWeightRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.WeightName) ? "" : request.WeightName), "weightName");
            var response = await client.PostAsync("/api/don-vi/tao-moi", requestContent);
            var body = await response.Content.ReadAsStringAsync();
            return body;
        }

        public async Task<string> Update(UpdateWeightRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.WeightName) ? "" : request.WeightName), "weightName");
            requestContent.Add(new StringContent(request.WeightId.ToString()), "weightId");
            var response = await client.PutAsync("/api/don-vi/cap-nhat", requestContent);
            var body = await response.Content.ReadAsStringAsync();
            return body;
        }

        public async Task<string> Delete(int WeightId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);

            var response = await client.DeleteAsync($"/api/don-vi/xoa/{WeightId}");
            var body = await response.Content.ReadAsStringAsync();
            return body;
        }

        public async Task<string> GetWeightById(int weightId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiUrl"]);

            var response = await client.GetAsync($"/api/don-vi/{weightId}");

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }
    }
}