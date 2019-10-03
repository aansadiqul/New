using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.CompanyTypes.Dto
{
    [AutoMapFrom(typeof(CompanyType))]
    public class CompanyTypeDto : AuditedEntityDto
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRetail { get; set; }
        public bool? IsWholesale { get; set; }
    }
}
