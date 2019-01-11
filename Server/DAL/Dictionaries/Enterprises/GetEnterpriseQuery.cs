using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Enterprises;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Enterprises
{
    public class GetEnterpriseQuery : QueryReader<Guid, EnterpriseDTO>
    {
        public GetEnterpriseQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            return @"select ENTERPRISE_ID, ENTERPRISE_NAME, IS_GR, CODE, Sort_order from rd.V_ENTERPRISES 
                    where hidden != 1 and ENTERPRISE_ID = :entId order by sort_order, ENTERPRISE_NAME";
        }


        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("entId", parameters);
        }

        protected override EnterpriseDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            EnterpriseDTO result = null;
            if (reader.Read())
            {
                result =  new EnterpriseDTO
                           {
                               Id = reader.GetValue<Guid>("ENTERPRISE_ID"),
                               Name = reader.GetValue<string>("ENTERPRISE_NAME"),
                               Code = reader.GetValue<string>("CODE"),
                               IsGr = reader.GetValue<bool?>("IS_GR"),
                               SortOrder = reader.GetValue<int>("Sort_order"),
                           };

            }
            return result;
        }
    }
}