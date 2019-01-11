using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.SerieChecks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.SerieChecks
{
    public class UpdateEntityPropertyTypeCommand : CommandNonQuery<UpdateEntityPropertyTypeParameterSet>
    {
        public UpdateEntityPropertyTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, UpdateEntityPropertyTypeParameterSet parameters)
        {
            command.AddInputParameter("p_entity_type_id", parameters.EntityTypeId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);

            if (parameters.IsMandatory.HasValue)
                command.AddInputParameter("p_is_mandatory", parameters.IsMandatory);

            if (parameters.IsInput.HasValue)
                command.AddInputParameter("p_is_input", parameters.IsInput);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(UpdateEntityPropertyTypeParameterSet parameters)
        {
            return "rd.P_ENTITY_TYPE_PROPERTY.Edit";
        }
    }
}



