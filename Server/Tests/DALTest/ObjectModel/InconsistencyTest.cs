using System;
using System.Linq;
using GazRouter.DAL.ObjectModel.Inconsistency;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Inconsistency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
	public class InconsistencyTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void FullTestInconsistency()
        {
			Guid newGuidSite = CreateSite();

			var inconsistencyId =
				new AddInconsistencyCommand(Context).Execute(new AddInconsistencyParameterSet
			                                                 {
																 EntityId = newGuidSite,InconsistencyTypeId = InconsistencyType.DiameterValveError
			                                                 });
			var inconsistency =
                new GetInconsistencyListQuery(Context).Execute(newGuidSite).First(p => p.Id == inconsistencyId);
			Assert.IsNotNull(inconsistency);
			new DeleteAllInconsistencyCommand(Context).Execute();
			var inconsistencyList =
				new GetInconsistencyListQuery(Context).Execute(null);
			Assert.IsTrue(inconsistencyList.Count == 0);

        }
    }
}
