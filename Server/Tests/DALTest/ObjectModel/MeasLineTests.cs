using System;
using System.Linq;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class MeasLineTests : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteMeasLine()
        {
            const string measLineName = "newLine";
            const string measStationName = "newStation";


            Guid siteId = CreateSite();

            Guid measStationId =
                new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
                    {
                        ParentId = siteId,
                        Name = measStationName,
                        BalanceSignId = Sign.In
                    });
            var measLinesBefore = new GetMeasLineListQuery(Context).Execute(new GetMeasLineListParameterSet { MeasStationId = measStationId }).Count;
            var pipelineId = CreatePipeline(PipelineType.Branch);



            new GetMeasStationListQuery(Context).Execute(null);

            var measLineId =
                new AddMeasLineCommand(Context).Execute(new AddMeasLineParameterSet
                    {
                        Name = measLineName,
                        PipelineId = pipelineId,
                        ParentId = measStationId
                    });
            var measLinesAfter = new GetMeasLineListQuery(Context).Execute(new GetMeasLineListParameterSet { MeasStationId = measStationId }).Count;

            Assert.AreEqual(measLinesBefore + 1, measLinesAfter);
            Assert.AreNotEqual(default(Guid), measLineId);


            const string bulshitName = "BulshitName";
            new EditMeasLineCommand(Context).Execute(new EditMeasLineParameterSet
                {
                    Id = measLineId,
                    Name = bulshitName,
                    PipelineId = pipelineId,
                    ParentId = measStationId
                });
            var tempmeasLinesAfter = new GetMeasLineListQuery(Context).Execute(new GetMeasLineListParameterSet { MeasStationId = measStationId });
            var measLineDto = new GetMeasLineByIdQuery(Context).Execute((tempmeasLinesAfter.First()).Id);
            Assert.IsNotNull(measLineDto);

            var editedMeasLIne = tempmeasLinesAfter.First();

            Assert.AreEqual(bulshitName, editedMeasLIne.Name);

            new DeleteMeasLineCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = measLineId,
                    EntityType = EntityType.MeasLine
                });
            tempmeasLinesAfter = new GetMeasLineListQuery(Context).Execute(new GetMeasLineListParameterSet { MeasStationId = measStationId });
            var delitedMeasLIine = tempmeasLinesAfter.FirstOrDefault();
            Assert.IsNull(delitedMeasLIine);
        }

        [TestMethod ,TestCategory(Stable)]
		public void AddEditDeleteMeasPoint()
		{
			{

				const string measLineName = "newLine";
				const string measStationName = "newStation";
           
                Guid newGuidSite =
                                  CreateSite();

				Guid measStationId =
                   new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
				   {
                       ParentId = newGuidSite,
					   Name = measStationName,
                       BalanceSignId = Sign.In
				   });
			    var pipelineId = CreatePipeline(PipelineType.Branch);
                   

				var measLineId =
                    new AddMeasLineCommand(Context).Execute(new AddMeasLineParameterSet
					{
						Name = measLineName,
						PipelineId = pipelineId,
						//MeasStationId = measStationIds[0].Id
						ParentId = measStationId

					});
				var measPointId =
                    new AddMeasPointCommand(Context).Execute(new AddMeasPointParameterSet { MeasLineId = measLineId, Name = "TestName" });
				Assert.AreNotEqual(default(Guid),measPointId);
				var measPointDto = new GetMeasPointByIdQuery(Context).Execute(measPointId);
                Assert.IsTrue(measPointDto != null);
                new DeleteMeasPointCommand(Context).Execute(new DeleteEntityParameterSet { Id = measPointId, EntityType = EntityType.MeasPoint });
                var measPointList = new GetMeasPointListQuery(Context).Execute(null);
				Assert.IsNull(measPointList.FirstOrDefault(p => p.Id == measPointId));
			}
		}
    }
}