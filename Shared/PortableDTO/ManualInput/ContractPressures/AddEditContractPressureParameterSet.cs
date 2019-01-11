using System;

namespace GazRouter.DTO.ManualInput.ContractPressures
{
    public class AddEditContractPressureParameterSet
    {
        public Guid distr_station_outlet_id { get; set; }

        public double? contract_pressure { get; set; }
    }
}
