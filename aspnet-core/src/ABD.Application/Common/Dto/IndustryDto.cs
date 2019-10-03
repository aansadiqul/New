using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Common.Dto
{
    public class IndustryDto
    {
        //public int Id;      
        //public string ParentId;
        public string Text;
        public string Code;
        public string SIC;
        public int? Level;
        public bool Expandable;       
        public string SICID;        
        public bool? IsChecked;

        public List<IndustryDto> children = new List<IndustryDto>();
    }

}
