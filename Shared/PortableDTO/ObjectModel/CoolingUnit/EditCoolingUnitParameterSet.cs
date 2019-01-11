using System;

namespace GazRouter.DTO.ObjectModel.CoolingUnit
{
	public class EditCoolingUnitParameterSet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
		public int CoolintUnitType { get; set; }
    }
}
