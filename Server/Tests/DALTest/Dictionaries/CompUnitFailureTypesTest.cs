using GazRouter.DAL.Dictionaries.AnnuledReasons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class CompUnitFailureTypesTest : TransactionTestsBase
    {
		[TestMethod, TestCategory(Stable)]
		public void AnnuledReasonsList()
        {
            var list = new GetAnnuledReasonsListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, dto => dto.AnnuledReason);
        }
    }
}