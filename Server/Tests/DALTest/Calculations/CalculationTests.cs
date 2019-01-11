using System;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.Calculations;
using GazRouter.DAL.Calculations.Calculation;
using GazRouter.DAL.Calculations.Log;
using GazRouter.DAL.Calculations.Parameter;
using GazRouter.DAL.EntitySelector;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Calculations.Log;
using GazRouter.DTO.Calculations.Parameter;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Calculations
{
    [TestClass]
    public class CalculationTests : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void ExpressionTest()
        {
            var input = @"Rez:=Var1*Var2;
/*dfghdfhdfhd*/Rez:=Rez+1000;/*вапвап*/
Rez:=Rez+1000
/*dfghdfhdfhd*/".Replace(Environment.NewLine, " ");
            
            var output =  Regex.Replace(input, @"(/\*(.*?)\*/)", string.Empty);

            Assert.AreEqual(@"Rez:=Var1*Var2; Rez:=Rez+1000; Rez:=Rez+1000 ", output);
        }

        [TestMethod ,TestCategory(Stable)]
        public void FullTest()
        {
            var entId = GetEnterpriseId();
            Assert.IsTrue(entId != default(Guid));
            var user = GetCurrentUser();
            new EditUserCommand(Context).Execute(new EditUserParameterSet
                {
                    Id = user.Id,
                    Description = user.Description,
                    UserName = user.UserName,
                    SiteId = entId
                });
            
            var calcId = new AddCalculationCommand(Context).Execute(new AddCalculationParameterSet
                {
                    //EventTypeId = 1,
                    PeriodTypeId = PeriodType.Day,
                    Description = "test",
                    SortOrder = 0,
                    Expression = "var1:=var2 + 1",
                    Sql = "sql",
                    SysName = "rd" + new Random().Next(100)
                });
            Assert.AreNotEqual(calcId, default(int));

            var calcsList = new GetCalculationListQuery(Context).Execute(new GetCalculationListParameterSet());
            AssertHelper.IsNotEmpty(calcsList);

            
            var entityIds = new GetEntityListQuery(Context).Execute(null).Select(e => e.Id);

            //var output = new GetCalcTestExecuteSqlQuery(Context).Execute(new GetCalcExecuteSqlParameterSet {CalculationId = calcId, HideError = true});

            var entityId = entityIds.FirstOrDefault();
            var calcParId = new AddCalculationParameterCommand(Context).Execute(new AddEditCalculationParameterParameterSet
                                                                                  {
                                                                                      CalculationId = calcId,
                                                                                      Alias = "var1",
                                                                                      ParameterTypeId = ParameterType.Out,
                                                                                      PropertyTypeId = PropertyType.PressureOutlet,
                                                                                      IsNumeric = true,
                                                                                      TimeShiftUnit = TimeShiftUnit.Mi,
                                                                                      TimeShiftValue = 10,
                                                                                      EntityId = entityId
                                                                                  });

            new EditCalculationParameterCommand(Context).Execute(new AddEditCalculationParameterParameterSet
                                                                     {
                                                                         Id = calcParId,
                                                                         CalculationId = calcId,
                                                                         Alias = "var2",
                                                                         ParameterTypeId = ParameterType.In,
                                                                         PropertyTypeId = PropertyType.PressureOutlet,
                                                                         IsNumeric = true,
                                                                         TimeShiftUnit = TimeShiftUnit.Mi,
                                                                         TimeShiftValue = 11,
                                                                         EntityId = entityId,
                                                                         Value = "0"
                                                                         
                                                                     });

            new GetCalculationParameterCommand(Context).Execute(new GetCalculationParameterParameterSet
            {
                Alias = "var2",
                SysName = "rd"
            });

            var calculationParList = new GetCalculationParameterListQuery(Context).Execute(calcId);
            AssertHelper.IsNotEmpty(calculationParList);

            new GetCalculationLogsQuery(Context).Execute(new GetLogListParameterSet());
            new GetCalculationLogsQuery(Context).Execute(new GetLogListParameterSet{CalculationId = calcId, StartDate =  DateTime.Now.AddDays(-1), EndDate =  DateTime.Now});

            new DeleteCalculationCommand(Context).Execute(calcId);
        }
    }
}