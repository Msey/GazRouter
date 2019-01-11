using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.SystemVariables;
using GazRouter.DTO.SystemVariables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest
{
    [TestClass]
    public class SystemVariablesTest : TransactionTestsBase
    {
        ///// <summary>
        ///// тест, возвращение списка системных переменных и их значений
        ///// </summary>
        //[TestMethod, TestCategory(UnStable)]
        //public void TestSystemVariableListGet()
        //{
        //    List<IusVariableDTO> sysValueList = new GetIusVariableListCommand(Context).Execute();
        //    AssertHelper.IsNotEmpty(sysValueList);
        //}


        //[TestMethod ,TestCategory(UnStable)]
        //public void TestSysVarModify()
        //{
        //    var sysValueList = new GetIusVariableListCommand(Context).Execute();
        //    AssertHelper.IsNotEmpty(sysValueList);

        //    var item = sysValueList.First();

        //    //изменение значения системной переменной
        //    new EditIusVariableCommand(Context).Execute(
        //        new IusVariableParameterSet
        //            {
        //                Name = item.Name,
        //                Value = "тестовое значение"
        //            });
        //}
    }
}
