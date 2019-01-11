using System;
using System.Collections.Generic;
using GazRouter.DTO.Balances.DayBalance;
using GazRouter.DTO.Balances.Docs;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.OperConsumers;
using GazRouter.DTO.Balances.Plan;

namespace DataProviders.Balances
{
    public class BalancesDataProvider : DataProviderBase<IBalancesService>
	{
        protected override string ServiceUri
        {
            get { return "/Balances/BalancesService.svc"; }

        }

        #region GasOwners

		public void GetGasOwnersList(int? parameters, Func<List<GasOwnerDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
			Execute(channel, channel.BeginGetGasOwnersList, channel.EndGetGasOwnersList, callback, parameters, behavior);
        }

		public void DeleteGasOwners(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
			Execute(channel, channel.BeginDeleteGasOwners, channel.EndDeleteGasOwners, callback, parameters, behavior);
        }

		public void EditSortOrderGasOwners(SetGasOwnerSortOrderParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginEditSortOrderGasOwners, channel.EndEditSortOrderGasOwners, callback, parameters, behavior);
		}

        #endregion

        #region Plan

        public void GetPlan(GetPlanParameterSet parameters, Func<PlanDto, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetPlan, channel.EndGetPlan, callback, parameters, behavior);
        }

        public void UpdateIntakesAndConsumptions(UpdatePlanParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
            Execute(channel, channel.BeginUpdatePlan, channel.EndUpdatePlan, callback, parameters, behavior);
		}

		#endregion

		#region DayBalance

		public void GetDayBalancesList(DateTime parameters, Func<DayBalancesDTO, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetDayBalancesList, channel.EndGetDayBalancesList, callback, parameters, behavior);
		}

		public void UpdateDayBalances(UpdateDayBalanceParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginUpdateDayBalances, channel.EndUpdateDayBalances, callback, parameters, behavior);
		}

		#endregion

		#region DayInfoBalance

        public void GetDayInfoBalancesList(DateTime parameters, Func<DayInfoBalancesDTO, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetDayInfoBalancesList, channel.EndGetDayInfoBalancesList, callback, parameters, behavior);
		}

		#endregion

        
        #region OperConsumers


        public void GetOperConsumers(Func<List<OperConsumerDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetOperConsumers, channel.EndGetOperConsumers, callback, behavior);
        }

        public void DeleteOperConsumers(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginDeleteOperConsumer, channel.EndDeleteOperConsumer, callback, parameters, behavior);
        }

        #endregion
        

		#region GasOwners2Consumer

		public void DeleteGasOwners2Consumer(BaseGasOwner2ConsumerParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginDeleteGasOwners2Consumer, channel.EndDeleteGasOwners2Consumer, callback, parameters, behavior);
		}

		public void AddGasOwners2Consumer(BaseGasOwner2ConsumerParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginAddGasOwners2Consumer, channel.EndAddGasOwners2Consumer, callback, parameters, behavior);
		}

		#endregion
    }
}
