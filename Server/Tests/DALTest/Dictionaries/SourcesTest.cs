using GazRouter.DAL.DataExchange.DataSource;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DTO.DataExchange.DataSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class SourcesTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void SourcesList()
        {
            var sourceId = new AddDataSourceCommand(Context).Execute(new AddDataSourceParameterSet(){Description = "desc", Name = "test", SysName = "test"});
            new EditDataSourceCommand(Context).Execute(new EditDataSourceParameterSet{Description = "desc", Name = "test", SysName = "test", Id = sourceId});
            new DeleteDataSourceCommand(Context).Execute(sourceId);
            var list = new GetSourcesListQuery(Context).Execute();
            AssertHelper.IsNotEmpty(list);
        }
    }
}