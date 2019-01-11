using System;
using System.Collections.Generic;

namespace GazRouter.DTO.Balances.Plan
{
    public class UpdatePlanParameterSet
	{
        public UpdatePlanParameterSet()
        {
            Intakes = new List<IntakeTransitItemDto>();
            CommercialIntakeCorrections = new List<IntakeTransitCorrectionDto>();
            OperativeIntakeCorrections = new List<IntakeTransitCorrectionDto>();
            IrregularIntakeCorrections = new List<IntakeTransitCorrectionDto>();

            Transits = new List<IntakeTransitItemDto>();
            CommercialTransitCorrections = new List<IntakeTransitCorrectionDto>();
            OperativeTransitCorrections = new List<IntakeTransitCorrectionDto>();
            IrregularTransitCorrections = new List<IntakeTransitCorrectionDto>();

            Consumptions = new List<ConsumptionItemDto>();
            CommercialConsumptionCorrections = new List<ConsumptionCorrectionDto>();
            OperativeConsumptionCorrections = new List<ConsumptionCorrectionDto>();
            IrregularConsumptionCorrections = new List<ConsumptionCorrectionDto>();

            OperConsumptions = new List<OperConsumptionItemDto>();
            CommercialOperConsumptionCorrections = new List<OperConsumptionCorrectionDto>();
            OperativeOperConsumptionCorrections = new List<OperConsumptionCorrectionDto>();
            IrregularOperConsumptionCorrections = new List<OperConsumptionCorrectionDto>();

            AuxConsumptions = new List<AuxConsumptionItemDto>();
            CommercialAuxConsumptionCorrections = new List<AuxConsumptionCorrectionDto>();
            OperativeAuxConsumptionCorrections = new List<AuxConsumptionCorrectionDto>();
            IrregularAuxConsumptionCorrections = new List<AuxConsumptionCorrectionDto>();
        }

        public int GasTransportSystemId { get; set; }
        public DateTime ContractDate { get; set; }

        public List<IntakeTransitItemDto> Intakes { get; set; }
        public List<IntakeTransitCorrectionDto> CommercialIntakeCorrections { get; set; }
        public List<IntakeTransitCorrectionDto> OperativeIntakeCorrections { get; set; }
        public List<IntakeTransitCorrectionDto> IrregularIntakeCorrections { get; set; }

        public List<IntakeTransitItemDto> Transits { get; set; }
        public List<IntakeTransitCorrectionDto> CommercialTransitCorrections { get; set; }
        public List<IntakeTransitCorrectionDto> OperativeTransitCorrections { get; set; }
        public List<IntakeTransitCorrectionDto> IrregularTransitCorrections { get; set; }

        public List<ConsumptionItemDto> Consumptions { get; set; }
        public List<ConsumptionCorrectionDto> CommercialConsumptionCorrections { get; set; }
        public List<ConsumptionCorrectionDto> OperativeConsumptionCorrections { get; set; }
        public List<ConsumptionCorrectionDto> IrregularConsumptionCorrections { get; set; }

        public List<OperConsumptionItemDto> OperConsumptions { get; set; }
        public List<OperConsumptionCorrectionDto> CommercialOperConsumptionCorrections { get; set; }
        public List<OperConsumptionCorrectionDto> OperativeOperConsumptionCorrections { get; set; }
        public List<OperConsumptionCorrectionDto> IrregularOperConsumptionCorrections { get; set; }

        public List<AuxConsumptionItemDto> AuxConsumptions { get; set; }
        public List<AuxConsumptionCorrectionDto> CommercialAuxConsumptionCorrections { get; set; }
        public List<AuxConsumptionCorrectionDto> OperativeAuxConsumptionCorrections { get; set; }
        public List<AuxConsumptionCorrectionDto> IrregularAuxConsumptionCorrections { get; set; }
    }

}
