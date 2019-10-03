using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.CompanyTypes.Dto
{
    //custom PagedResultRequestDto
    public class GetTypesInput : IPagedResultRequest
    {
        public const int DefaultPageSize = 10;

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public string Keyword { get; set; }

        public bool? IsActive { get; set; }

        public GetTypesInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
