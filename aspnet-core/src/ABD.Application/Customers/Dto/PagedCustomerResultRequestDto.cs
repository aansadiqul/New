using Abp.Application.Services.Dto;
using System;

namespace ABD.Customers.Dto
{
    //custom PagedResultRequestDto
    public class PagedCustomerResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public long? RoleId { get; set; }
    }
}
