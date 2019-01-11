using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Enterprises;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Enterprises
{
    public class GetEnterpriseExchangeNeighbourList : QueryReader<Guid, List<EnterpriseDTO>>
    {
        public GetEnterpriseExchangeNeighbourList(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      e.enterprise_id, 
                                    e.enterprise_name, 
                                    e.is_gr, 
                                    e.code, 
                                    e.sort_order 
                        FROM        v_enterprises e 
                        WHERE       e.enterprise_id IN 
                            (SELECT     neighbour_enterprise_id 
                                FROM    v_enterprise_neighbours 
                                WHERE   enterprise_id = :id)
                            AND e.is_gr = 1
                        ORDER BY    enterprise_name";
        }


        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override List<EnterpriseDTO> GetResult(OracleDataReader reader, Guid parameters)
        {
            var result = new List<EnterpriseDTO>();
            while (reader.Read())
            {
                result.Add(new EnterpriseDTO
                           {
                               Id = reader.GetValue<Guid>("enterprise_id"),
                               Name = reader.GetValue<string>("enterprise_name"),
                               Code = reader.GetValue<string>("code"),
							   SortOrder = reader.GetValue<int>("sort_order"),
                           });

            }
            return result;
        }

    }
}