using System;
using System.Collections.Generic;
using GazRouter.DTO.Repairs;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;

namespace DataProviders.Repairs
{
    public class RepairsDataProvider : DataProviderBase<IRepairsService>
	{
        protected override string ServiceUri
        {
            get { return "/Repairs/RepairsService.svc"; }

        }

        #region Plan
        
        public void GetRepairPlan(GetRepairPlanParameterSet parameters, Func<RepairPlanDataDTO, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetRepairPlan, channel.EndGetRepairPlan, callback, parameters, behavior);
        }

        public void SetPlanningStage(SetPlanningStageParameterSet parameters, Func<Exception, bool> callback,
            IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginSetPlanningStage, channel.EndSetPlanningStage, callback, parameters, behavior);
        }

        #endregion



        #region Complexes

        public void ComplexListGet(GetRepairPlanParameterSet parameters, Func<List<ComplexDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetComplexList, channel.EndGetComplexList, callback, parameters, behavior);
        }
        

        public void ComplexDelete(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginDeleteComplex, channel.EndDeleteComplex, callback, parameters, behavior);
        }

        public void AddRepairToComplex(AddRepairToComplexParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddRepairToComplex, channel.EndAddRepairToComplex, callback, parameters, behavior);
        }

        public void MoveComplex(EditComplexParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginMoveComplex, channel.EndMoveComplex, callback, parameters, behavior);
        }

        #endregion

		

		#region Repaires

        public void DeleteRepair(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
            Execute(channel, channel.BeginDeleteRepair, channel.EndDeleteRepair, callback, parameters, behavior);
		}

        public void EditRepairDates(EditRepairDatesParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginEditRepairDates, channel.EndEditRepairDates, callback, parameters, behavior);
        }

        public void GetKilometerList(Guid parameters, Func<List<double>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetKilometerList, channel.EndGetKilometerList, callback, parameters, behavior);
		}

        public void GetRepairUpdateHistory(int parameters, Func<List<RepairUpdateDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetRepairUpdateHistory, channel.EndGetRepairUpdateHistory, callback, parameters, behavior);
        }

		#endregion

    }
}
