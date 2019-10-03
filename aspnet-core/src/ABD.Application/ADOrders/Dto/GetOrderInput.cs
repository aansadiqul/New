﻿using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.ADOrders.Dto
{
    public class GetOrderInput : IPagedResultRequest
    {
        public const int DefaultPageSize = 10;

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public string Keyword { get; set; }

        public GetOrderInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
