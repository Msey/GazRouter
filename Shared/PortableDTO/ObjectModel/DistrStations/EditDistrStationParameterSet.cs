namespace GazRouter.DTO.ObjectModel.DistrStations
{
    public class EditDistrStationParameterSet : EditEntityParameterSet
    {
        public int RegionId { get; set; }
        public double? PressureRated { get; set; }
        public double? CapacityRated { get; set; }

        public bool IsForeign { get; set; }
    }
}