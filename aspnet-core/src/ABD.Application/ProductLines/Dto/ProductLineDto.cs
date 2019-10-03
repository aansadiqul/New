using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.ProductLines.Dto
{
    [AutoMapFrom(typeof(ProductLine))]
    public class ProductLineDto : AuditedEntityDto
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
