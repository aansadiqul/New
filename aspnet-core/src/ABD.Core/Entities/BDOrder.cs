using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ABD.Authorization.Users;

namespace ABD.Entities
{
    public class BDOrder : FullAuditedEntity<int>
    {
        [Column("OrderID")]
        [Key]
        public override int Id { get; set; }
        [Required]
        public int SearchID { get; set; }
        [StringLength(100)]
        public string SalesUser { get; set; }
        [StringLength(150)]
        public string Description { get; set; }
        public int? RecordCount { get; set; }
        public int? CreditsUsed { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? RecordPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? OrderValue { get; set; }
        [StringLength(50)]
        public string PaymentType { get; set; }
        [StringLength(50)]
        public string CCTransactionId { get; set; }
        [StringLength(50)]
        public string CCAuthorization { get; set; }
        [StringLength(50)]
        public string CheckNumber { get; set; }
        public bool? Active { get; set; }
        public string OrderNotes { get; set; }
        public string CCNum { get; set; }
        public bool? XDatesPurchased { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? XDateOrdValue { get; set; }
        public int? XDatesPurchasedCnt { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? XDatesOrderAmt { get; set; }
    }
}
