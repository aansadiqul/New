using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.BDSearches.Dto
{
    public class GetBDSearchInput : IPagedResultRequest
    {
        public const int DefaultPageSize = 10;

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public string Keyword { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public GetBDSearchInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
