using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Calculations.Parameter;

namespace GazRouter.Modes.Calculations.Dialogs.GetCalculationsByVar
{
    public class GetCalculationsByVarViewModel : DialogViewModel
    {
        
        public GetCalculationsByVarViewModel(CalculationParameterDTO varDto) : base(null)
        {
            GetCalcList(varDto);
        }

        
        public List<CalculationDTO> CalcList { get; set; }

        private async void GetCalcList(CalculationParameterDTO dto)
        {
            Lock();

            CalcList = await new CalculationServiceProxy().GetCalculationListAsync(
                new GetCalculationListParameterSet
                {
                        EntityId = dto.EntityId,
                        PropertyType = dto.PropertyTypeId,
                        ParameterType = dto.ParameterTypeId
                });
            OnPropertyChanged(() => CalcList);

            Unlock();
        }
    }
}