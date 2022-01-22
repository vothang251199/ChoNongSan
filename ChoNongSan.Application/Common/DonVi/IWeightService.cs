using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.DonVi;
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

        Task<bool> Create(CreateWeightRequest request, WeightType weightExist);

        Task<bool> Update(UpdateWeightRequest request, WeightType weightExist);

        Task<bool> Delete(WeightType weightExist);

        WeightVm GetWeightById(WeightType weight);
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
            var data = await _context.WeightTypes.AsNoTracking().Where(x => x.IsDelete == false).ToListAsync();
            var lsWeight = data.Select(x => new WeightVm()
            {
                WeightId = x.WeightId,
                WeightName = x.WeightName,
            }).ToList();
            return lsWeight;
        }

        public async Task<bool> Create(CreateWeightRequest request, WeightType weightExist)
        {
            try
            {
                if (weightExist != null)
                {
                    weightExist.WeightName = request.WeightName;
                    weightExist.IsDelete = false;
                    _context.WeightTypes.Update(weightExist);
                }
                else
                {
                    var weight = new WeightType()
                    {
                        WeightName = request.WeightName,
                        IsDelete = false,
                    };
                    _context.WeightTypes.Add(weight);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Update(UpdateWeightRequest request, WeightType weightExist)
        {
            try
            {
                weightExist.WeightName = request.WeightName;
                _context.WeightTypes.Update(weightExist);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(WeightType weightExist)
        {
            try
            {
                weightExist.IsDelete = true;
                _context.WeightTypes.Update(weightExist);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public WeightVm GetWeightById(WeightType weight)
        {
            var result = new WeightVm()
            {
                WeightId = weight.WeightId,
                WeightName = weight.WeightName,
            };
            return result;
        }
    }
}