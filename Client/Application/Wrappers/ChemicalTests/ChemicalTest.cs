using GazRouter.DTO.ManualInput.ChemicalTests;
using GazRouter.DTO.ObjectModel;
using Utils.Units;

namespace GazRouter.Application.Wrappers.ChemicalTests
{
    public class ChemicalTest
    {
        public ChemicalTest(ChemicalTestDTO dto)
        {
            Dto = dto;
        }

        public ChemicalTestDTO Dto { get; }

        public CommonEntityDTO ParentEntityDto => new CommonEntityDTO {Id = Dto.ParentId, Name = Dto.ParentName, ShortPath = Dto.ParentShortPath, EntityType = Dto.ParentEntityType};

        public ChemicalTestWarning DewPointHydrocarbonWarning
        {
            get
            {
                // летний период с 1.05 - 30.09
                // зимний период с 1.10 - 30.04
                var norm = Temperature.FromCelsius( (Dto.TestDate.HasValue && (Dto.TestDate.Value.Month >= 5 && Dto.TestDate.Value.Month <= 9))
                    ? (Dto.IsFrigid ? -5 : -2)
                    : (Dto.IsFrigid ? -10 : -2));
                

                return new ChemicalTestWarning
                {
                    IsActive = Dto.DewPointHydrocarbon > norm.Celsius,
                    Message =
                        $"Превышает норму {norm.ToString(UserProfile.Current.UserSettings.TemperatureUnit)}, см. СТО Газпром 089-2010"
                };
            }
        }

        public ChemicalTestWarning DewPointWarning
        {
            get
            {
                // летний период с 1.05 - 30.09
                // зимний период с 1.10 - 30.04
                var norm = Temperature.FromCelsius((Dto.TestDate.HasValue && (Dto.TestDate.Value.Month >= 5 && Dto.TestDate.Value.Month <= 9))
                    ? (Dto.IsFrigid ? -14 : -10)
                    : (Dto.IsFrigid ? -20 : -10));

                return new ChemicalTestWarning
                {
                    IsActive = Dto.DewPoint > norm.Celsius,
                    Message =
                        $"Превышает норму {norm.ToString(UserProfile.Current.UserSettings.TemperatureUnit)}, см. СТО Газпром 089-2010."
                };
            }
        }
        

        public ChemicalTestWarning SourSulfurWarning
        {
            get
            {
                return new ChemicalTestWarning
                {
                    IsActive = Dto.ConcentrSourSulfur > 0.016,
                    Message = "Превышает норму 0.016 г/м³, см. СТО Газпром 089-2010."
                };
            }
        }

        public ChemicalTestWarning HydrogenSulfideWarning
        {
            get
            {
                return new ChemicalTestWarning
                {
                    IsActive = Dto.ConcentrHydrogenSulfide > 0.007,
                    Message = "Превышает норму 0.007 г/м³, см. СТО Газпром 089-2010."
                };
            }
        }

        public ChemicalTestWarning CarbonDioxidWarning
        {
            get
            {
                return new ChemicalTestWarning
                {
                    IsActive = Dto.ContentCarbonDioxid > 2.5,
                    Message = "Превышает норму 2.5 %, см. СТО Газпром 089-2010."
                };
            }
        }

        public ChemicalTestWarning CombHeatWarning
        {
            get
            {
                return new ChemicalTestWarning
                {
                    IsActive = Dto.CombHeatLow < 7600,
                    Message = "Меньше нормы 7600 ккал/м³, см. СТО Газпром 089-2010."
                };
            }
        }
    }
}