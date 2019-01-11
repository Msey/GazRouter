using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel.CompShops;
using Telerik.Windows.Controls.Data.PropertyGrid.Converters;

namespace GazRouter.Application.Wrappers.Entity
{
    public class CompShopWrapper : EntityWrapperBase<CompShopDTO>
    {
        public CompShopWrapper(CompShopDTO dto, bool displaySystem)
            : base(dto, displaySystem)
        {
            AddProperty("Тип",
                Enumerable.Single<EngineClassDTO>(ClientCache.DictionaryRepository.EngineClasses, c => c.EngineClass == dto.EngineClass).Name);
            AddProperty("Газопровод подключения", dto.PipelineName);
            AddProperty("Километр подключения", dto.KmOfConn.HasValue ? dto.KmOfConn.Value.ToString("0.###") : "");

            //AddProperty("Геом. объем коммуникаций цеха, тыс.м³",
            //    dto.PipingVolume.HasValue ? dto.PipingVolume.Value.ToString("0.###") : "");

            AddProperty("Геом. объем входных коммуникаций цеха, тыс.м³",
                dto.PipingVolumeIn.HasValue ? dto.PipingVolumeIn.Value.ToString("0.###") : "");
            AddProperty("Геом. объем выходных коммуникаций цеха, тыс.м³",
                dto.PipingVolumeOut.HasValue ? dto.PipingVolumeOut.Value.ToString("0.###") : "");
        }

        [Display(Name = "Тип", Order = 10)]
        public string EngineClass
        {
            get
            {
                return
                    Enumerable.Single<EngineClassDTO>(ClientCache.DictionaryRepository.EngineClasses, c => c.EngineClass == _dto.EngineClass).Name;
            }
        }

        [Display(Name = "Газопровод подключения", Order = 20)]
        public string PipelineName
        {
            get { return _dto.PipelineName; }
        }

        [Display(Name = "Километр подключения", Order = 30)]
        public double? KmOfConn
        {
            get { return _dto.KmOfConn; }
        }

        [Display(Name = "Геом. объем коммуникаций цеха, тыс.м³", Order = 40)]
        [TypeConverter(typeof (NumericTypeConverter))]
        public double? PipingVolume
        {
            get { return _dto.PipingVolume; }
        }

        

        [Display(Name = "Виртуальный", Order = 50)]
        public bool IsVirtual
        {
            get { return _dto.IsVirtual; }
        }
    }
}