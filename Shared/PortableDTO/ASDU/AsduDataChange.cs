using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ASDU
{
    public class AsduDataChange
    {
        public int Nrownum { get; set; }
        public string Cchangetype { get; set;  }
        public string Cclass { get; set; }
        public string Cobjname { get; set; }
        public string Cobjid { get; set; }
        public string Cparamname { get; set; }
        public string Cdesc { get; set; }
    }
}
