using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Customers.Dto;
using ABD.Entities;

namespace ABD.ADOrders.Dto
{
    public class AdReceiptDto 
    {
        public CustomerDto Neilson { get; set; }
        public CustomerDto Customer { get; set; }
        public ADOrderDto Order { get; set; }
    }
}
