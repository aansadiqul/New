using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;

namespace ABD.Payment.Dto
{
    public class GetPaymentInput : IPagedResultRequest
    {
        public const int DefaultPageSize = 10;

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public string Keyword { get; set; }

        public GetPaymentInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
