using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.ContactTitles.Dto
{
    [AutoMapTo(typeof(ContactTitle))]
    public class CreateContactTitleInput
    {
        [Required]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
