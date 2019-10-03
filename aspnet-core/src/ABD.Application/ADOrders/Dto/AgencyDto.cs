using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.ADOrders.Dto
{
    [AutoMapTo(typeof(Agency))]
    public class AgencyDto : EntityDto
    {
        public string Account { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string County { get; set; }
        public string TimeZone { get; set; }
        public string Country { get; set; }
        public string Division { get; set; }
        public string MainPhone { get; set; }
        public string PhoneExtension { get; set; }
        public string Fax { get; set; }
        public string TollFree { get; set; }
        public string WebAddress { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public decimal? Revenue { get; set; }
        public int? PercentComm { get; set; }
        public int? Employees { get; set; }
        public string SpecialAffiliation { get; set; }
        public string AgencyManagement { get; set; }
        public string Producer { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Suffix { get; set; }
        public string Title { get; set; }
        public string Mobile { get; set; }
        public string CEmail { get; set; }
        public string AccountId { get; set; }
        public string SpLines { get; set; }
        public string TitleSearch { get; set; }
        public string LineSearch { get; set; }
        public string Msa { get; set; }
        public double? PremiumVolume { get; set; }
        public string DunsNum { get; set; }
        public string CEmail2 { get; set; }
        public string LinkedUrl { get; set; }
        public string BranchIndicator { get; set; }
        public int? Num_Locations { get; set; }
        public string TwitterUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string GoogleplusUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string Linkedin { get; set; }
        public string CountyCode { get; set; }
        public string CTwitterUrl { get; set; }
        public string CFacebookUrl { get; set; }
        public string CGoogleplusUrl { get; set; }
        public string CYoutubeUrl { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int? GeoCodeStatus { get; set; }
    }
}
