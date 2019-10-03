using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace ABD.ADOrders.Dto
{
    public class ReportResultDto<T> : ListResultDto<T>
    {
        public OrderStatus Status { get; set; }
        public ReportResultDto(OrderStatus status, List<T> items)
            : base(items)
        {
            Status = status;
        }
    }
}
