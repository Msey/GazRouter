using System.Collections.Generic;
using GazRouter.DAL.SystemVariables;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.SystemVariables;

namespace GazRouter.DataServices.SystemVariables
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class SysVarService : ServiceBase, ISysVarService
    {
        public List<IusVariableDTO> GetIusVariableList()
        {
            return ExecuteRead<GetIusVariableListQuery, List<IusVariableDTO>>();
        }

        public void EditIusVariableValue(IusVariableParameterSet newValue)
        {
            ExecuteNonQuery<EditIusVariableCommand, IusVariableParameterSet>(newValue);
        }

        public string GetHelpFileName()
        {
            return Infrastructure.AppSettingsManager.HelpFileRelativeUri;
        }

        public string GetLastChangesFileName()
        {
            return Infrastructure.AppSettingsManager.LastChangesFileRelativeUri;
        }
    }
}
