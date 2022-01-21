using ChoNongSan.Application.Common.DonVi;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.DonVi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost("tao-moi")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateWeightRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var weightExist = await _context.WeightTypes.AsNoTracking().FirstOrDefaultAsync(x => x.WeightName.ToLower().Contains(request.WeightName.ToLower()));
            if (weightExist != null)
            {
                if (weightExist.IsDelete == false)
                {
                    return BadRequest(new { message = "Đơn vị đã tồn tại", status = "FAILED" });
                }
            }
            var result = await _weightService.Create(request, weightExist);
            if (result == true)
                return Ok(new { message = "Tạo đơn vị thành công", status = "OK" });
            return BadRequest(new { message = "Tạo đơn vị thất bại", status = "FAILED" });
        }

        [HttpPut("cap-nhat")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateWeightRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var weightExist = await _context.WeightTypes.AsNoTracking().FirstOrDefaultAsync(x => x.WeightId == request.WeightId);
            if (weightExist == null)
            {
                return BadRequest(new { message = "Đơn vị không tồn tại", status = "FAILED" });
            }

            var result = await _weightService.Update(request, weightExist);
            if (result == true)
                return Ok(new { message = "Cập nhật đơn vị thành công", status = "OK" });
            return BadRequest(new { message = "Cập nhật đơn vị thất bại", status = "FAILED" });
        }

        [HttpDelete("xoa/{WeightId}")]
        public async Task<IActionResult> Delete([FromRoute] int WeightId)
        {
            var weightExist = await _context.WeightTypes.FindAsync(WeightId);
            if (weightExist == null)
            {
                return BadRequest(new { message = "Đơn vị không tồn tại", status = "FAILED" });
            }

            var result = await _weightService.Delete(weightExist);
            if (result == true)
                return Ok(new { message = "Xóa đơn vị thành công", status = "OK" });
            return BadRequest(new { message = "Xóa đơn vị thất bại", status = "FAILED" });
        }

        [HttpGet("{weightId}")]
        public async Task<IActionResult> GetWeightById([FromRoute] int weightId)
        {
            var weight = await _context.WeightTypes.AsNoTracking().FirstOrDefaultAsync(x => x.WeightId == weightId && x.IsDelete == false);
            if (weight == null) return BadRequest(new { message = "Không tìm thấy đơn vị", status = "FAILED" });

            var result = _weightService.GetWeightById(weight);
            return Ok(new { data = result, status = "OK" });
        }
    }
}