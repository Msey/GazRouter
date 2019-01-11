using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Routes.Exceptions;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes
{

    public class GetRouteListQuery : QueryReader<GetRouteListParameterSet, List<RouteDTO>>
	{
        public GetRouteListQuery(ExecutionContext context)
			: base(context)
		{ }


        protected override string GetCommandText(GetRouteListParameterSet parameters)
        {
            var q = @"  SELECT      r.route_id,
                                    r.inlet_id,
                                    r.outlet_id,
                                    oe.entity_name AS outlet_name,
                                    oe.entity_type_id AS outlet_type_id,                                    
                                    r.length, 
                                    r.length_cmp,
                                    ex.route_exception_id,
                                    ex.gas_owner_id AS ex_gas_owner_id,
                                    go.gas_owner_name AS ex_gas_owner_name,
                                    ex.length AS ex_length                                    
                        FROM        v_bl_routes r
                        INNER JOIN  v_meas_stations ims ON ims.meas_station_id = r.inlet_id
                        INNER JOIN  v_entities oe ON oe.entity_id = r.outlet_id
                        LEFT JOIN   v_bl_route_exceptions ex ON ex.route_id = r.route_id
                        LEFT JOIN   v_gas_owners go ON go.gas_owner_id = ex.gas_owner_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters?.InletId != null)
                sb.Append(" AND r.inlet_id = :inletId");

            if (parameters?.SystemId != null)
                sb.Append(" AND ims.system_id = :systemId");


            sb.Append(" ORDER BY r.route_id");

            return sb.ToString();
        }

        protected override List<RouteDTO> GetResult(OracleDataReader reader, GetRouteListParameterSet parameters)
		{
            var routes = new List<RouteDTO>();

            RouteDTO route = null;

            while (reader.Read())
            {
                var routeId = reader.GetValue<int>("route_id");

                if (route?.RouteId != routeId)
                {
                    route = new RouteDTO
                    {
                        RouteId = routeId,
                        InletId = reader.GetValue<Guid>("inlet_id"),
                        OutletId = reader.GetValue<Guid>("outlet_id"),
                        OutletName = reader.GetValue<string>("outlet_name"),
                        OutletType = reader.GetValue<EntityType>("outlet_type_id"),
                        Length = reader.GetValue<double?>("length"),
                        CalcLength = reader.GetValue<double?>("length_cmp")
                    };
                    routes.Add(route);
                }

                var exceptionId = reader.GetValue<int?>("route_exception_id");
                if (exceptionId.HasValue)
                {
                    var exeception = new RouteExceptionDTO
                    {
                        RouteId = routeId,
                        RouteExceptionId = exceptionId.Value,
                        OwnerId = reader.GetValue<int>("ex_gas_owner_id"),
                        OwnerName = reader.GetValue<string>("ex_gas_owner_name"),
                        Length = reader.GetValue<double>("ex_length"),
                    };
                    route.ExceptionList.Add(exeception);
                }
            }
            return routes;
		}

        

        protected override void BindParameters(OracleCommand command, GetRouteListParameterSet parameters)
        {
            if (parameters?.InletId != null)
                command.AddInputParameter("inletId", parameters.InletId);

            if (parameters?.SystemId != null)
                command.AddInputParameter("systemId", parameters.SystemId);
        }
	}
}
