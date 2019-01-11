using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.DTO.Balances.Plan
{
    [DataContract]
    public class PlanDto
    {
        public PlanDto()
        {
            GasOwners = new List<GasOwnerDTO>();
            IntakeMeasStations = new List<MeasStationDTO>();
            TransitMeasStations = new List<MeasStationDTO>();
            Consumers = new List<ConsumerDTO>();
            OperConsumers = new List<OperConsumerDTO>();
            DistrStations = new List<DistrStationDTO>();
            Sites = new List<SiteDTO>();
            
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

        [DataMember]
        public ContractDTO Contract { get; set; }
        [DataMember]
        public List<GasOwnerDTO> GasOwners { get; set; }
        [DataMember]
        public List<MeasStationDTO> IntakeMeasStations { get; set; }
        [DataMember]
        public List<MeasStationDTO> TransitMeasStations { get; set; }
        [DataMember]
        public List<ConsumerDTO> Consumers { get; set; }
        [DataMember]
        public List<OperConsumerDTO> OperConsumers { get; set; }
        [DataMember]
        public List<DistrStationDTO> DistrStations { get; set; }
        [DataMember]
        public List<SiteDTO> Sites { get; set; }

        [DataMember]
        public List<IntakeTransitItemDto> Intakes { get; set; }
        [DataMember]
        public List<IntakeTransitCorrectionDto> CommercialIntakeCorrections { get; set; }
        [DataMember]
        public List<IntakeTransitCorrectionDto> OperativeIntakeCorrections { get; set; }
        [DataMember]
        public List<IntakeTransitCorrectionDto> IrregularIntakeCorrections { get; set; }

        [DataMember]
        public List<IntakeTransitItemDto> Transits { get; set; }
        [DataMember]
        public List<IntakeTransitCorrectionDto> CommercialTransitCorrections { get; set; }
        [DataMember]
        public List<IntakeTransitCorrectionDto> OperativeTransitCorrections { get; set; }
        [DataMember]
        public List<IntakeTransitCorrectionDto> IrregularTransitCorrections { get; set; }

        [DataMember]
        public List<ConsumptionItemDto> Consumptions { get; set; }
        [DataMember]
        public List<ConsumptionCorrectionDto> CommercialConsumptionCorrections { get; set; }
        [DataMember]
        public List<ConsumptionCorrectionDto> OperativeConsumptionCorrections { get; set; }
        [DataMember]
        public List<ConsumptionCorrectionDto> IrregularConsumptionCorrections { get; set; }

        [DataMember]
        public List<OperConsumptionItemDto> OperConsumptions { get; set; }
        [DataMember]
        public List<OperConsumptionCorrectionDto> CommercialOperConsumptionCorrections { get; set; }
        [DataMember]
        public List<OperConsumptionCorrectionDto> OperativeOperConsumptionCorrections { get; set; }
        [DataMember]
        public List<OperConsumptionCorrectionDto> IrregularOperConsumptionCorrections { get; set; }

        [DataMember]
        public List<AuxConsumptionItemDto> AuxConsumptions { get; set; }
        [DataMember]
        public List<AuxConsumptionCorrectionDto> CommercialAuxConsumptionCorrections { get; set; }
        [DataMember]
        public List<AuxConsumptionCorrectionDto> OperativeAuxConsumptionCorrections { get; set; }
        [DataMember]
        public List<AuxConsumptionCorrectionDto> IrregularAuxConsumptionCorrections { get; set; }
    }

    [DataContract]
    public abstract class PlanItemDto<TPointKey>
    {
        [DataMember]
        public int GasOwnerId { get; set; }
        [DataMember]
        public TPointKey PointId { get; set; }
        [DataMember]
        public long Value { get; set; }
        [DataMember]
        public BalanceValueType ValueType { get; set; }
    }

    [DataContract]
    public class IntakeTransitItemDto : PlanItemDto<Guid>
    {
    }

    [DataContract]
    public class ConsumptionItemDto : PlanItemDto<Guid>
    {
    }

    [DataContract]
    public class OperConsumptionItemDto : PlanItemDto<int>
    {
    }

    [DataContract]
    public class AuxConsumptionItemDto : PlanItemDto<Guid>
    {
    }

    [DataContract]
    public class IntakeTransitCorrectionDto : IntakeTransitItemDto, ICorrection
    {
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public int? DocId { get; set; }
        [DataMember]
        public string FileDescription { get; set; }
    }

    [DataContract]
    public class ConsumptionCorrectionDto : ConsumptionItemDto, ICorrection
    {
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public int? DocId { get; set; }
        [DataMember]
        public string FileDescription { get; set; }
    }

    [DataContract]
    public class OperConsumptionCorrectionDto : OperConsumptionItemDto, ICorrection
    {
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public int? DocId { get; set; }
        [DataMember]
        public string FileDescription { get; set; }
    }

    [DataContract]
    public class AuxConsumptionCorrectionDto : AuxConsumptionItemDto, ICorrection
    {
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public int? DocId { get; set; }
        [DataMember]
        public string FileDescription { get; set; }
    }

    public interface ICorrection
    {
        int GasOwnerId { get; set; }
        long Value { get; set; }
        BalanceValueType ValueType { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        int? DocId { get; set; }
        string FileDescription { get; set; }
    }

    public enum BalanceValueType
    {
        Base = 1,
        Commercial = 2,
        Operative = 3,
        Irregularity = 4
    }
}
