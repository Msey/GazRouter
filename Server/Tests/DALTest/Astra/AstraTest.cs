using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.SeriesData.GasInPipes;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.SeriesData.GasInPipes;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Astra
{
	[TestClass]
    public class AstraTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
        public void AddDeleteAstraTests()
		{
		    var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            var pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                {
                    Name = "Pipeline1",
                    PipelineTypeId = PipelineType.Bridge,
                    Hidden = true,
                    SortOrder = 1,
                    KilometerOfStart = 0.23,
                    KilometerOfEnd = 0.43,
                    GasTransportSystemId = gastransport.Id
                });

            var serieId = new AddSeriesCommand(Context).Execute(new AddSeriesParameterSet
            {
                PeriodTypeId = PeriodType.Twohours,
                KeyDate = DateTime.Now,
                Description = string.Empty
            });

		    new AddGasInPipeCommand(Context).Execute(new AddGasInPipeParameterSet
		                                                      {
                                                                  SeriesId = serieId,
                                                                  PipelineId = pipelineId,
                                                                  Value = 1,
                                                                  StartKm = 1,
                                                                  EndKm = 2,
                                                                  Description = null
		                                                      });
            new DeleteGasInPipeCommand(Context).Execute(serieId);
		}
	}
}
