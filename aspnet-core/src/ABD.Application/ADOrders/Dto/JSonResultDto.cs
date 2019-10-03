using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace ABD.ADOrders.Dto
{
    public class JSonResultDto
    {
        public OrderStatus Status { get; set; }
        public string Data { get; set; }

        public JSonResultDto(OrderStatus status, string items)
        {
            Status = status;
            Data = items;
        }
    }
}
