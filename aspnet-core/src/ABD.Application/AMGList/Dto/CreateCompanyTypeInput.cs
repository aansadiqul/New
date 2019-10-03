using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.AMGList.Dto
{
    [AutoMapTo(typeof(Entities.AMGList))]
    public class CreateAMGListInput
    {
        [Required]
        public string AList { get; set; }
        public bool? IsActive { get; set; }
    }
}
