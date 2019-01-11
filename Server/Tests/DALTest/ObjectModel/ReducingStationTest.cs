using System.Linq;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.ReducingStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class ReducingStationTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestReducingStations()
        {
            var siteId = CreateSite();
            var pipelineId = CreatePipeline(PipelineType.Branch);
            const double kilometerOfStartAfterCreation = 23;
            var reducingStationId =
                new AddReducingStationCommand(Context).Execute(new AddReducingStationParameterSet
                    {
                        Name = "ReducingStation1",
                        Hidden = false,
                        Status = false,
                        SortOrder = 1,
                        SiteId = siteId,
                        MainPipelineId = pipelineId,
                        Kilometer = kilometerOfStartAfterCreation
                    });

            var getReducingStation =
                new GetReducingStationListQuery(Context).Execute(null);
            AssertHelper.IsNotEmpty(getReducingStation);

            var reducingDto =
                new GetReducingStationByIdQuery(Context).Execute((getReducingStation.First()).Id);
            Assert.IsNotNull(reducingDto);

            const string bulshitName = "asdfasdfasfdasfdf";
            const double kilometerOfStartAfterEditing = 24;
            new EditReducingStationCommand(Context).Execute(new EditReducingStationParameterSet
                {
                    ReducingStationId = reducingStationId,
                    Name = bulshitName,
                    Hidden = false,
                    Status = false,
                    SortOrder = 1,
                    SiteId = siteId,
                    MainPipelineId = pipelineId,
                    Kilometer = kilometerOfStartAfterEditing
                });

            var reducingStation = new GetReducingStationByIdQuery(Context).Execute(reducingStationId);

            Assert.AreEqual(bulshitName, reducingStation.Name);
            Assert.AreEqual(kilometerOfStartAfterEditing, reducingStation.Kilometer);

            new DeleteReducingStationCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = reducingStationId,
                    EntityType = EntityType.ReducingStation
                });

            var deletedPipline =
                new GetReducingStationByIdQuery(Context).Execute(reducingStationId);
            Assert.IsNull(deletedPipline);
        }
    }
}
