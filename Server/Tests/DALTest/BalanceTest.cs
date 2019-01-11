//using System;
//using System.Collections.Generic;
//using System.Linq;
//using DAL.Balances;
//using DAL.Core;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace DALTest
//{
//    [TestClass]
//    public class BalanceTest
//    {
//        [TestMethod ,TestCategory(Stable)]
//        public void TestGetGazBalanceValues()
//        {
//            ICollection<BalanceRowDTO> rows;
//            using (var context = OpenDbContext())
//            {
//                rows = new LoadBalancesCommand(context).Execute(new LoadBalancesParameterSet{ Date = new DateTime(2011, 11, 30), PlanCode = "P01" });
//            }
//            //Assert.IsTrue(rows != null && rows.Count > 0);
//            //Assert.AreEqual(31, rows.First().Cells.Count);
//        }

//    }
//}
