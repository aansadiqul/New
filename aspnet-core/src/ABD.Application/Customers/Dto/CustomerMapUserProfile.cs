using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ABD.Entities;
using Abp.Application.Services.Dto;

namespace ABD.Customers.Dto
{
    class CustomerMapUserProfile : Profile
    {
        public CustomerMapUserProfile()
        {
            CreateMap<CustomerDto, UserProfile>();
            CreateMap<CustomerDto, UserProfile>()
                .ForMember(x => x.Active, opt => opt.Ignore())
                .ForMember(x => x.CreationTime, opt => opt.Ignore());
        }
        
    }
}
