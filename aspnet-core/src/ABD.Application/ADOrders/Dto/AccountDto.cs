using System;
using System.Collections.Generic;
using System.Text;
using ABD.Entities;

namespace ABD.ADOrders.Dto
{
    public class AccountDto
    {
        public AgencyDto AgencyDetails { get; set; }
        public List<TargetSectorDto> TargetSectors { get; set; }
        public List<AffiliationDto> SpecialAffiliations { get; set; }
        public List<CarrierDto> Carriers { get; set; }

    }
}
