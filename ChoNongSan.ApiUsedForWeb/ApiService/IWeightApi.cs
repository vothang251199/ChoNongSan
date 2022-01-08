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
    }
}