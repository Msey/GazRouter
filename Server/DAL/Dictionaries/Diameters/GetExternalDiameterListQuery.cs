using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Diameters;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace GazRouter.DAL.Dictionaries.Diameters
{
    public class GetExternalDiameterListQuery : QueryReader<List<ExternalDiameterDTO>>
    {
        public GetExternalDiameterListQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText()
        {
            return
                @"select 
                    diametrs_external_id, diametr_id, diametr_external, wall_thickness
                    from v_diametrs_external 
                    ORDER BY diametr_external, wall_thickness";
        }

        protected override List<ExternalDiameterDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<ExternalDiameterDTO>();
            while (reader.Read())
            {
                var extDiam = reader.GetValue<double>("diametr_external");
                var thickness = reader.GetValue<double>("wall_thickness");

                var diameter =
                   new ExternalDiameterDTO
                   {
                       Id = reader.GetValue<int>("diametrs_external_id"),
                       InternalDiameterId = reader.GetValue<int>("diametr_id"),
                       Name = $"{extDiam} / {thickness}",
                       ExternalDiameter = extDiam,
                       WallThickness = thickness,
                   };
                result.Add(diameter);
            }
            return result;
        }
    }
}
