using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Positions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Positions
{
    public class GetVersionedPositionListQuery : QueryReader<int, List<PositionDTO>>
    {
        public GetVersionedPositionListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("versionId", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return @"SELECT entity_id, x, y, km FROM rd.V_POSITIONS where scheme_version_id = :versionId";
        }

        protected override List<PositionDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<PositionDTO>();
            while (reader.Read())
            {
                result.Add(new PositionDTO
                               {
                                   EntityId = reader.GetValue<Guid>("ENTITY_ID"),
                                   X = reader.GetValue<Double>("X"),
                                   Y = reader.GetValue<Double>("Y"),
                                   Km = reader.GetValue<Double>("KM"),
                               });

            }
            return result;
        }
    }
}