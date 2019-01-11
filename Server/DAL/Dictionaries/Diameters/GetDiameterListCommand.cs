using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Diameters;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Diameters
{
    public class GetDiameterListQuery : QueryReader<List<DiameterDTO>>
    {
        public GetDiameterListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return
                @"  SELECT      diametr_id,
                                diametr_name,
                                diameter_conv,
                                diameter_real
                    FROM        V_DIAMETRS 
                    ORDER BY    diameter_conv";
        }

        protected override List<DiameterDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<DiameterDTO>();
            while (reader.Read())
            {
                var diameter =
                   new DiameterDTO
                   {
                       Id = reader.GetValue<int>("diametr_id"),
                       Name = reader.GetValue<string>("diametr_name"),
                       DiameterConv = reader.GetValue<double>("diameter_conv"),
                       DiameterReal = reader.GetValue<double>("diameter_real"),
                   };
                result.Add(diameter);
            }
            return result;
        }
    }
}
