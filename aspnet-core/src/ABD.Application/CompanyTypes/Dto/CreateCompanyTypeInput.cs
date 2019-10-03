using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.CompanyTypes.Dto
{
    [AutoMapTo(typeof(CompanyType))]
    public class CreateCompanyTypeInput
    {
        [Required]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRetail { get; set; }
        public bool? IsWholesale { get; set; }
    }
}
