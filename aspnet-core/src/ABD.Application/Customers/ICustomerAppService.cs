using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services;
using ABD.Customers.Dto;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using ABD.Domain.Dtos;
using Microsoft.AspNetCore.Http;

namespace ABD.Customers
{
    public interface ICustomerAppService: IAsyncCrudAppService<CustomerDto, int, GetCustomerInput, CreateCustomerDto, CustomerDto>
    {
        Task <CustomerDto> GetCustomerByUserid(long id);
        Task<List<SalespersonDto>> GetSalesperson();
        Task<PagedResultDto<UserProfileDto>> GetAllCustomers(PagedCustomerResultRequestDto input);
        Task<PagedResultDto<UserProfileDto>> GetUsers(PagedCustomerResultRequestDto input);
        Task PostImage(IFormFile file, long userId);
    }
}
