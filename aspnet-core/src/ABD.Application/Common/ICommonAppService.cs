using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using ABD.Common.Dto;
using ABD.Domain.Dtos;

namespace ABD.Common
{
    public interface ICommonAppService : IApplicationService
    {
        Task<List<CommonDto>> GetStates();
        Task<List<CommonDto>> GetCarriers();
        Task<List<CommonDto>> GetAffiliations();
        Task<List<CommonDto>> GetContactTitles();
        Task<List<CommonDto>> GetProductLines();
        Task<List<CommonDto>> GetCompanyTypes();
        Task<List<CommonDto>> GetCounties(GetStatesInput input, bool isAdSearch = true);
        Task<ZipCodesDto> GetZipCodes(GetStatesInput input);
        Task<List<CommonDto>> GetMetroAreas(GetStatesInput input);
        Task<List<CommonDto>> GetAreas(GetStatesInput input);
        Task<List<IndustryDto>> GetIndustries();
    }
}
