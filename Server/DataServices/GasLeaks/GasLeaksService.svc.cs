using System.Collections.Generic;
using GazRouter.DAL.GasLeaks;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.GasLeaks;

namespace GazRouter.DataServices.GasLeaks
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class GasLeaksService : ServiceBase, IGasLeaksService
    {
    	public List<LeakDTO> GetLeaks(GetLeaksParameterSet parameters)
    	{
    	    var result = ExecuteRead<GetLeaksQuery, List<LeakDTO>, GetLeaksParameterSet>(parameters);
    	    return result;
    	}

        public void DeleteLeak(int parameters)
        {
            ExecuteNonQuery<DeleteLeakCommand, int>(parameters);
        }

        public int AddLeak(AddLeakParameterSet parameters)
        {
            return ExecuteRead<AddLeakCommand, int, AddLeakParameterSet>(parameters);
        }

        public void EditLeak(EditLeakParameterSet parameters)
        {
            ExecuteNonQuery<EditLeakCommand, EditLeakParameterSet>(parameters);
        }
    }
}
