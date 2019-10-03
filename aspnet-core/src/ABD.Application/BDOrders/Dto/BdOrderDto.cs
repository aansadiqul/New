using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.BDOrders.Dto
{
    [AutoMapFrom(typeof(BDOrder))]
    public class BdOrderDto : FullAuditedEntityDto
    {
        public int SearchID { get; set; }
        public string SalesUser { get; set; }
        public string Description { get; set; }
        public int? RecordCount { get; set; }
        public int? CreditsUsed { get; set; }
        public decimal? RecordPrice { get; set; }
        public decimal? OrderValue { get; set; }
        public string PaymentType { get; set; }
        public string CCTransactionId { get; set; }
        public string CCAuthorization { get; set; }
        public string CheckNumber { get; set; }
        public bool? Active { get; set; }
        public string OrderNotes { get; set; }
        public string CCNum { get; set; }
        public bool? XDatesPurchased { get; set; }
        public decimal? XDateOrdValue { get; set; }
        public int? XDatesPurchasedCnt { get; set; }
        public decimal? XDatesOrderAmt { get; set; }
    }
}
