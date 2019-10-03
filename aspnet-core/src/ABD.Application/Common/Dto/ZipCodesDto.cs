using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Domain.Dtos;
using ABD.Entities;

namespace ABD.Common.Dto
{
    public class ZipCodesDto
    {
        public List<CommonDto> Zip3Digits { get; set; }
        public List<CommonDto> Zip5Digits { get; set; }
    }
}
