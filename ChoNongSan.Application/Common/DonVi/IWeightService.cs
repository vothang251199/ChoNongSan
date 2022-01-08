using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common.DonVi
{
    public interface IWeightService
    {
        Task<List<WeightVm>> GetListWeight();
    }

    public class WeightService : IWeightService
    {
        public readonly ChoNongSanContext _context;

        public WeightService(ChoNongSanContext context)
        {
            _context = context;
        }

        public async Task<List<WeightVm>> GetListWeight()
        {
            var data = await _context.WeightTypes.ToListAsync();
            var lsWeight = data.Select(x => new WeightVm()
            {
                WeightId = x.WeightId,
                WeightName = x.WeightName,
            }).ToList();
            return lsWeight;
        }
    }
}