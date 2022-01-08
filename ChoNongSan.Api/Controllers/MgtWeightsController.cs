using ChoNongSan.Application.Common.DonVi;
using ChoNongSan.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/don-vi")]
    [ApiController]
    public class MgtWeightsController : ControllerBase
    {
        private readonly ChoNongSanContext _context;
        private readonly IWeightService _weightService;

        public MgtWeightsController(ChoNongSanContext context, IWeightService weightService)
        {
            _weightService = weightService;
            _context = context;
        }

        [HttpGet("tat-ca")]
        public async Task<IActionResult> GetListWeight()
        {
            return Ok(await _weightService.GetListWeight());
        }
    }
}