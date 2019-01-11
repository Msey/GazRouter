using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.StatesModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.States
{
	public class GetStateListQuery : QueryReader<StateSet, List<StateBaseDTO>>
	{
        public GetStateListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(StateSet parameters)
		{
            return @"   SELECT      to_number(row_id) as row_id,
                                    name
                        FROM        rd.v_dict_rows
                        WHERE       dict_id = :dictid
                        ORDER BY    sort_order";
		}

        protected override void BindParameters(OracleCommand command, StateSet parameters)
        {
            command.AddInputParameter("dictid", parameters);
        }


        protected override List<StateBaseDTO> GetResult(OracleDataReader reader, StateSet parameters)
		{
            var result = new List<StateBaseDTO>();
			while (reader.Read())
			{
				result.Add(new StateBaseDTO
                {
                    Id = reader.GetValue<int>("row_id"),
                    Name = reader.GetValue<string>("name"),
                    //Description = reader.GetValue<string>("description"),
				});
			}
			return result;
		}
	}
}