using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.Common.Dto
{
    [AutoMapFrom(typeof(RsState))]
    public class StatesDto : EntityDto
    {
        public string State { get; set; }
        public string Description { get; set; }
    }
}
