using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.Affiliations.Dto
{
    [AutoMapTo(typeof(SpecialAffiliation))]
    public class CreateAffiliationInput
    {
        [Required]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
