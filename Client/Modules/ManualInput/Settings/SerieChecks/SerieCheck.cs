using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.SeriesData.SerieChecks;

namespace GazRouter.ManualInput.Settings.SerieChecks
{
    public class SerieCheck
    {
        public SerieCheck(SerieCheckDTO dto)
        {
            Dto = dto;
        }

        public SerieCheckDTO Dto { get; set; }

        public bool IsEnable
        {
            get { return Dto.IsEnabled; }
            set
            {
                if (Dto.IsEnabled != value)
                {
                    Dto.IsEnabled = value;

                    new SeriesDataServiceProxy().UpdateSerieCheckAsync(
                        new UpdateSerieCheckParameterSet
                        {
                            CheckId = Dto.Id,
                            IsEnable = value
                        });
                }
            }
        }

        public bool IsEnabled { get; set; }

    }
}