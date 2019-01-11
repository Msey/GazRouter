using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using GazRouter.DTO.DataExchange.ExchangeTask;
namespace GazRouter.DAL.DataExchange.TypicalExchange
{
    public class GetTypicalExchangeTaskList: QueryReader<Guid , List<NeighbourEnterpriseExchangeTask>>
    {
        public GetTypicalExchangeTaskList(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            return @"
SELECT      t1.neighbour_enterprise_id,
            t2.enterprise_name,
            t2.is_gr,
            t3.exchange_task_id
FROM        v_enterprise_neighbours t1
Join        v_enterprises t2 on t2.enterprise_id = t1.neighbour_enterprise_id
Left Join   v_exchange_tasks t3 on t3.enterprise_id = t2.enterprise_id and t3.exchange_type_id = 2   -- экспорт 
Where       t1.Enterprise_id = :p_enterprise_id
            and t2.is_gr = 1               
                ";
        }       

        protected override void BindParameters(OracleCommand command, Guid parameter)
        {
             command.AddInputParameter("p_enterprise_id", parameter);
            
        }

        protected override List<NeighbourEnterpriseExchangeTask> GetResult(OracleDataReader reader, Guid parameters)
        {
            var tasks = new List<NeighbourEnterpriseExchangeTask>();
            while (reader.Read())
            {
                tasks.Add(new NeighbourEnterpriseExchangeTask
                {
                    Name = reader.GetValue<string>("enterprise_name"),
                    Id = reader.GetValue<Guid>("neighbour_enterprise_id"),
                    ExchangeTaskId = reader.GetValue<int?>("exchange_task_id")

                });
            }
            return tasks;
        }
    }
}
   




