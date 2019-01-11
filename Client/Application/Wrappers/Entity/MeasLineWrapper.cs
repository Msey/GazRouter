using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.ObjectModel.MeasLine;

namespace GazRouter.Application.Wrappers.Entity
{
    public class MeasLineWrapper : EntityWrapperBase<MeasLineDTO>
    {
        public MeasLineWrapper(MeasLineDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Газопровод", dto.PipelineName);
            AddProperty("Километр подключения", dto.KmOfConn.ToString("0.###"));
        }


        [Display(Name = "Газопровод", Order = 10)]
        public string PipelineName
        {
            get { return _dto.PipelineName; }
        }

        [Display(Name = "Километр подключения", Order = 20)]
        public double KmOfConn
        {
            get { return _dto.KmOfConn; }
        }

        
    }
}