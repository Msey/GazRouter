using System;
using System.Collections.Generic;
using GazRouter.DAL.GasLeaks;
using GazRouter.DTO.GasLeaks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest
{
    [TestClass]
    public class GasLeakTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestGetLeaks()
        {
            Guid enterpriseId = GetEnterpriseId(); 
            
            new AddLeakCommand(Context).Execute(
                new AddLeakParameterSet
                    {
                        Place = "TestPlace",
                        Reason = "TestReason",
                        VolumeDay = 0,
                        RepairActivity = "TestRepairActivity",
                        Description = "TestDescription",
                        ContactName = "TestContactName",
                        DiscoverDate = DateTime.Now,
                        RepairPlanDate = DateTime.Now,
                        RepairPlanFact = DateTime.Now,
                        EntityId = enterpriseId
                    });

            var startDate = new DateTime(2011, 12, 30, 0, 0, 0, DateTimeKind.Local);
            List<LeakDTO> leaks = new GetLeaksQuery(Context).Execute(
                new GetLeaksParameterSet
                    {
                        StartDate = startDate,
                        EndDate = DateTime.Now
                    });
            AssertHelper.IsNotEmpty(leaks );
        }

        [TestMethod ,TestCategory(Stable)]
        public void TestModifyLeak()
        {
            {
				Guid newGuid =
                    GetEnterpriseId();
                var id = new AddLeakCommand(Context).Execute(
                    new AddLeakParameterSet
                        {
                            Place = "TestPlace",
                            Reason = "TestReason",
                            VolumeDay = 0,
                            RepairActivity = "TestRepairActivity",
                            Description = "TestDescription",
                            ContactName = "TestContactName",
                            DiscoverDate = DateTime.Now,
                            RepairPlanDate = DateTime.Now,
                            RepairPlanFact = DateTime.Now,
							EntityId = newGuid
                        });

                var leak = new GetLeakByIdQuery(Context).Execute(id);

                Assert.IsNotNull(leak);

                new EditLeakCommand(Context).Execute(
                    new EditLeakParameterSet
                {
                    LeakId = id,
                    Place = "TestPlace1",
                    Reason = "TestReason1",
                    VolumeDay = 0,
                    RepairActivity = "TestRepairActivity1",
                    Description = "TestDescription1",
                    ContactName = "TestContactName",
                    DiscoverDate = DateTime.Now,
                    RepairPlanDate = DateTime.Now,
                    RepairPlanFact = DateTime.Now,
					EntityId = newGuid
                });

                leak = new GetLeakByIdQuery(Context).Execute(id);

                Assert.AreEqual(leak.Place, "TestPlace1");

                new DeleteLeakCommand(Context).Execute(id);

                 leak = new GetLeakByIdQuery(Context).Execute(id);

                Assert.IsNull(leak);

            }
        }


    }
}
