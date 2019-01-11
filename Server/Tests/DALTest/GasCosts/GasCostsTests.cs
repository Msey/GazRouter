using System;
using System.Linq;
using GazRouter.DAL.GasCosts;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.Extensions;

namespace DALTest.GasCosts
{
      [TestClass]
    public class GasCostsTests : DalTestBase
    {
          [TestMethod ,TestCategory(Stable)]
          public void CrudTest()
          {
              var siteId = CreateSite();
              var compShopId = GetCompShop(Context, siteId);

              const int calculatedVolume = 4;
              const int measuredVolume = 5;
              DateTime date = DateTime.Today;
              var listCostType = new GetCostTypeListQuery(Context).Execute();
              Assert.IsTrue(listCostType.Any());
              var costType = listCostType.First().CostType;
              int costId = new AddGasCostCommand(Context).Execute(new AddGasCostParameterSet
                 {
                      EntityId = compShopId,
                      CostType = costType,
                      Date = date,
                      CalculatedVolume = calculatedVolume,
                      MeasuredVolume = measuredVolume,
                      Target = Target.Fact,
                      SiteId = siteId
                     
                  });

              var gasCost = new GetGasCostListQuery(Context).Execute(new GetGasCostListParameterSet { Target = Target.Fact, StartDate = date.MonthStart(), EndDate = date.MonthEnd()}).SingleOrDefault(c => c.Id == costId);

             Assert.IsNotNull(gasCost);
             Assert.AreEqual(compShopId, gasCost.Entity.Id);
             Assert.AreEqual(costType, gasCost.CostType);
             Assert.AreEqual(date, gasCost.Date);
             Assert.AreEqual(calculatedVolume, gasCost.CalculatedVolume);
             Assert.AreEqual(measuredVolume, gasCost.MeasuredVolume);

              const int newCalculatedVolume = 12;
              const int newMeasuredVolume = 0;
              new EditGasCostCommand(Context).Execute(new EditGasCostParameterSet
              {
                      CostId = costId,
                      EntityId = compShopId,
                      CostType = costType,
                      Date = date,
                      CalculatedVolume = newCalculatedVolume,
                      MeasuredVolume = newMeasuredVolume,
                      Target = Target.Plan,
                      SiteId = siteId

                  });


               gasCost = new GetGasCostListQuery(Context).Execute(new GetGasCostListParameterSet {Target =  Target.Plan }).SingleOrDefault(c => c.Id == costId);

              Assert.IsNotNull(gasCost);
              Assert.AreEqual(compShopId, gasCost.Entity.Id);
              Assert.AreEqual(costType, gasCost.CostType);
              Assert.AreEqual(date, gasCost.Date);
              Assert.AreEqual(newCalculatedVolume, gasCost.CalculatedVolume);
              Assert.AreEqual(newMeasuredVolume, gasCost.MeasuredVolume);

              new DeleteGasCostCommand(Context).Execute(costId);

              gasCost = new GetGasCostListQuery(Context).Execute(new GetGasCostListParameterSet()).SingleOrDefault(c => c.Id == costId);

              Assert.IsNull(gasCost);


              // Доступ
              new SetGasCostAccessCommand(Context).Execute(
                  new SetGasCostAccessParameterSet
                  {
                      Date = date.MonthStart(),
                      SiteId = siteId,
                      Target = Target.Fact,
                      IsRestricted = true,
                      PeriodType = GazRouter.DTO.Dictionaries.PeriodTypes.PeriodType.Month
                  });

              
              var acl =
              new GetGasCostAccessListQuery(Context).Execute(
                  new GetGasCostAccessListParameterSet
                  {
                      Date = date.MonthStart(),
                      SiteId = siteId
                  });

              Assert.IsNotNull(acl);
              Assert.IsTrue(acl.Count > 0);
              var ac = acl.Single(a => a.SiteId == siteId);
              Assert.IsNotNull(ac);
              Assert.AreEqual(ac.Fact, false);


          }

    }
}