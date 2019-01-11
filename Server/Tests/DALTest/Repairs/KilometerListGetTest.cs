using System.Linq;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.Repairs.Repair;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Repairs
{
    [TestClass]
    public class KilometerListGetTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestRepairTypeListGet()
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

            Assert.AreNotEqual(default(int), pipelineId);

            new KilommetrListGetQuery(Context).Execute(pipelineId);
        }
    }
}