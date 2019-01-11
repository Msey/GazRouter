using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.SuperchargerTypes
{
    public class GetSuperchargerTypesQuery : QueryReader<List<SuperchargerTypeDTO>>
    {
        public GetSuperchargerTypesQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return
                @"SELECT    supercharger_type_id, 
                            supercharger_type_name,
                            e_rated, 
                            k_a_rated, 
                            q_max, 
                            q_min, 
                            r_rated, 
                            t_rated, 
                            rpm_rated, 
                            z_rated, 
                            n_cbn_rated
                  FROM      v_supercharger_types";
        }

        protected override List<SuperchargerTypeDTO> GetResult(OracleDataReader reader)
        {
            var periodTypes = new List<SuperchargerTypeDTO>();
            while (reader.Read())
            {
                var periodType =
                    new SuperchargerTypeDTO
                    {
                        Id = reader.GetValue<int>("supercharger_type_id"),
                        Name = reader.GetValue<string>("supercharger_type_name"),
                        ERated = reader.GetValue<double?>("e_rated"),
                        KaRated = reader.GetValue<double?>("k_a_rated"),
                        QMax = reader.GetValue<double?>("q_max"),
                        QMin = reader.GetValue<double?>("q_min"),
                        RRated = reader.GetValue<double?>("r_rated"),
                        TRated = reader.GetValue<double?>("t_rated"),
                        RpmRated = reader.GetValue<double?>("rpm_rated"),
                        ZRated = reader.GetValue<double?>("z_rated"),
                        NCbnRated = reader.GetValue<double?>("n_cbn_rated")
                    };
                periodTypes.Add(periodType);
            }
            return periodTypes;
        }
    }
}