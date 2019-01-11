using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Parameter;


namespace GazRouter.Modes.Calculations
{
    public class VarItem : ViewModelBase
    {
        public VarItem(CalculationParameterDTO dto)
        {
            Dto = dto;
        }

        public CalculationParameterDTO Dto { get; set; }
        public string Value
        {
            get { return Dto.Value; }
            set
            {
                Dto.Value = value;
                OnPropertyChanged(() => Value);
            }
        }

        /// <summary>
        /// Используется ли параметр в других расчетах
        /// </summary>
        public bool IsUsingInOtherCalc => Dto.UseCount > 0;

        
        //public string PropertyName
        //{
        //    get
        //    {
        //        return Dto.PropertyTypeId.HasValue ?
        //            ClientCache.DictionaryRepository.PropertyTypes.First(p => p.PropertyType == Dto.PropertyTypeId).Name
        //            : string.Empty;
        //    }
        //}

        //public string PropertyPath
        //{
        //    get
        //    {
        //        return Dto.PropertyTypeId.HasValue ?
        //            Dto.Path + ". "
        //            + ClientCache.DictionaryRepository.PropertyTypes.First(p => p.PropertyType == Dto.PropertyTypeId).Name
        //            : string.Empty;
        //    }
        //}
    
    }
}