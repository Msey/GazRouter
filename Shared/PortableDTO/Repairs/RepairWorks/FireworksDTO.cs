using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Repairs.RepairWorks
{
    public class FireworksDTO
    {
        public enum FireTypes
        {
            FireWork = 1,
            GasWork = 2,
            WorkWork = 3,
            OtherWork = 4
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            return ((FireworksDTO)obj).Firetype == Firetype;
        }

        public FireTypes Firetype { get; set; } = FireTypes.OtherWork;

        public string Name
        {
            get
            {
                switch (Firetype)
                {
                    case FireTypes.FireWork: return "Огневые работы";
                    case FireTypes.GasWork: return "Газоопасные работы";
                    case FireTypes.WorkWork: return "Работы без вывода оборудования в ремонт";
                    case FireTypes.OtherWork: return "Прочие работы";
                }
                return "";
            }
        }

        public static List<FireworksDTO> GetList()
        {
            return new List<FireworksDTO>() {
                new FireworksDTO() { Firetype = FireTypes.FireWork},
                new FireworksDTO() { Firetype = FireTypes.GasWork },
                new FireworksDTO() { Firetype = FireTypes.WorkWork},
                new FireworksDTO() { Firetype = FireTypes.OtherWork }
            };
        }
    }
}
