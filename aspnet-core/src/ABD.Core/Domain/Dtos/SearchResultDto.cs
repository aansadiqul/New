using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.Domain.Dtos
{
    public class SearchResultDto
    {
        public int AgencyList { get; set; }
        public int Contacts { get; set; }
        public int ContactsWithEmail { get; set; }
    }
}
