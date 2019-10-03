using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ABD.Authorization.Users;

namespace ABD.Customers.Dto
{
    public class UserProfileDto : EntityDto<long>
    {
        
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime CreationTime { get; set; }
        public int[] RoleIds{ get; set; }
        public DateTime? AdActiveDate { get; set; }
        public DateTime? AdExpireDate { get; set; }
        public DateTime? BdActiveDate { get; set; }
        public string SalesUser { get; set; }
        public string CompanyName { get; set; }
        public string ImagePath { get; set; }
        public bool? IsSalesPerson { get; set; }

    }
}
