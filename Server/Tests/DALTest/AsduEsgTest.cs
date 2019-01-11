//using System;
//using System.Collections.Generic;
//using DAL.AsduEsg;
//using DAL.Core;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace DALTest
//{
//    [TestClass]
//    public class AsduTest
//    {
//        [TestMethod ,TestCategory(Stable)]
//        public void TestHourSessionGetCommand()
//        {
//            ICollection<SessionValue> sessionValueList;
//            using (var context = OpenDbContext())
//            {
//                sessionValueList = AsduCommandFactory.CreateGetHourSessionDataCommand(context).Execute(new DateTime(2011, 8, 8, 12, 0, 0));
//            }
//            Assert.IsTrue(sessionValueList != null && sessionValueList.Count > 0);
//        }

//    }
//}
