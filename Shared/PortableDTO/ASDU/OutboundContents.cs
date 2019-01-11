using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ASDU
{
    public class OutboundContents
    {
        public int Key { get; set; }
        public string Nodetype { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Changestate { get; set; }
        public string Value { get; set; }
        public string ValueAsdu { get; set; }
    }
}
