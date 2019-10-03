using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.Carriers.Dto
{
    [AutoMapFrom(typeof(CarrierLine))]
    public class CarrierLineDto : AuditedEntityDto
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
