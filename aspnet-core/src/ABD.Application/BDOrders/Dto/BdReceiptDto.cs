using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Customers.Dto;
using ABD.Entities;

namespace ABD.BDOrders.Dto
{
    public class BdReceiptDto 
    {
        public CustomerDto Neilson { get; set; }
        public CustomerDto Customer { get; set; }
        public BdOrderDto Order { get; set; }
    }
}
