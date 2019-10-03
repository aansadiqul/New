using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ABD.AMGList.Dto
{
    [AutoMapTo(typeof(Entities.AMGList))]
    public class AMGListDto : AuditedEntityDto
    {
        public string AList { get; set; }
        public bool? IsActive { get; set; }
    }
}
