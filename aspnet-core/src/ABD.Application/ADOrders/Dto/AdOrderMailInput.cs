using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.ADOrders.Dto
{
    public class AdOrderMailInput
    {
        public int OrderId { get; set; }
        public string Description{ get; set; }
        public DateTime OrderDate { get; set; }
        public string SalesUser { get; set; }
        public int? RecordCount{ get; set; }
        public int? CtCount{ get; set; }
        public string CustomerName { get; set; }
        public string PaymentType { get; set; }
        public string CheckNo { get; set; }
        public string CardNumber { get; set; }
        public decimal? OrderValue { get; set; }
        public string TargetUser { get; set; }
    }
}
