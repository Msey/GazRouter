using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Wrappers.ChemicalTests;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.ManualInput.ChemicalTests;


namespace GazRouter.Controls.Dialogs.ObjectDetails.ChemicalTests
{
    public class ChemicalTestsViewModel : ValidationViewModel
    {
        private readonly Guid _entityId;
        
        public ChemicalTestsViewModel(Guid id)
        {
            _entityId = id;

            LoadTests();
        }


        public List<GridItem> Items { get; set; }

        public bool HasTest { get; set; }

        private async void LoadTests()
        {
            try
            {
                Behavior.TryLock();

                var testDto =
                    (await
                        new ManualInputServiceProxy().GetChemicalTestListAsync(new GetChemicalTestListParameterSet
                        {
                            ParentId = _entityId
                        })).FirstOrDefault();

                if (testDto == null)
                {
                    HasTest = false;
                    OnPropertyChanged(() => HasTest);
                    return;
                }

                HasTest = true;
                OnPropertyChanged(() => HasTest);

                var test = new ChemicalTest(testDto);

                Items = new List<GridItem>();
                Items.Add(new GridItem
                {
                    Name = "Дата проведения анализа",
                    Value = string.Format("{0:dd.MM.yyyy}{1}{0:HH:mm}", test.Dto.TestDate, Environment.NewLine)
                });
                
                Items.Add(new GridItem
                {
                    Name = "Т точки росы по влаге",
                    Eu = UserProfile.UserUnitName(PhysicalType.Temperature),
                    Value = test.Dto.DewPoint.HasValue ?
                        UserProfile.ToUserUnits(test.Dto.DewPoint.Value, PhysicalType.Temperature).ToString("0.#") : "",
                    HasWarnings = test.DewPointWarning.IsActive,
                    WarningMessage = test.DewPointWarning.Message
                });
                
                Items.Add(new GridItem
                {
                    Name = "Т точки росы по углеводородам",
                    Eu = UserProfile.UserUnitName(PhysicalType.Temperature),
                    Value = test.Dto.DewPointHydrocarbon.HasValue ?
                             UserProfile.ToUserUnits(test.Dto.DewPointHydrocarbon.Value, PhysicalType.Temperature).ToString("0.#") :""
                });

                Items.Add(new GridItem
                {
                    Name = "Содержание (мол.доля) азота",
                    Eu = "%",
                    Value = test.Dto.ContentNitrogen.HasValue ? test.Dto.ContentNitrogen.Value.ToString("0.#") : ""
                });

                Items.Add(new GridItem
                {
                    Name = "Содержание (мол.доля) CO2",
                    Eu = "%",
                    Value = test.Dto.ContentCarbonDioxid.HasValue ? test.Dto.ContentCarbonDioxid.Value.ToString("0.#") : "",
                    HasWarnings = test.CarbonDioxidWarning.IsActive,
                    WarningMessage = test.CarbonDioxidWarning.Message
                });

                Items.Add(new GridItem
                {
                    Name = "Массовая концентрация меркаптановой серы",
                    Eu = "г/м³",
                    Value = test.Dto.ConcentrSourSulfur.HasValue ? test.Dto.ConcentrSourSulfur.Value.ToString("0.#") : "",
                    HasWarnings = test.SourSulfurWarning.IsActive,
                    WarningMessage = test.SourSulfurWarning.Message
                });

                Items.Add(new GridItem
                {
                    Name = "Массовая концентрация сероводорода",
                    Eu = "г/м³",
                    Value = test.Dto.ConcentrHydrogenSulfide.HasValue ? test.Dto.ConcentrHydrogenSulfide.Value.ToString("0.#") : "",
                    HasWarnings = test.HydrogenSulfideWarning.IsActive,
                    WarningMessage = test.HydrogenSulfideWarning.Message
                });

                Items.Add(new GridItem
                {
                    Name = "Плотность",
                    Eu = "кг/м³",
                    Value = test.Dto.Density.HasValue ? test.Dto.Density.Value.ToString("0.#") : ""
                });

                Items.Add(new GridItem
                {
                    Name = "Теплота сгорания газа низшая",
                    Eu = "ккал/м³",
                    Value = test.Dto.CombHeatLow.HasValue ? UserProfile.ToUserUnits(test.Dto.CombHeatLow.Value, PhysicalType.CombustionHeat).ToString("0.#") : "",
                    HasWarnings = test.CombHeatWarning.IsActive,
                    WarningMessage = test.CombHeatWarning.Message
                });

                OnPropertyChanged(() => Items);

            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }
    }

    public class GridItem
    {
        public string Name { get; set; }

        public string Eu { get; set; }

        public string Value { get; set; }

        public bool HasWarnings { get; set; }

        public string WarningMessage { get; set; }
    }
}