using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.ObjectModel.ReducingStations;

namespace GazRouter.Application.Wrappers.Entity
{
    public class ReducingStationWrapper : EntityWrapperBase<ReducingStationDTO>
    {
        public ReducingStationWrapper(ReducingStationDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Газопровод", dto.PipelineName);
            AddProperty("Километр", dto.Kilometer.ToString("0.###"));
        }

        [Display(Name = "Газопровод", Order = 10)]
        public string PipelineName
        {
            get { return _dto.PipelineName; }
        }

        [Display(Name = "Километр", Order = 20)]
        public double Kilometer
        {
            get { return _dto.Kilometer; }
        }
    }
}