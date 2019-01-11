using System;
using System.Collections.Generic;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Bindings.EntityBindings;
using GazRouter.DAL.Bindings.EntityPropertyBindings;
using GazRouter.DAL.Bindings.PropertyBindings;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
using GazRouter.DTO.Bindings.PropertyBindings;
using GazRouter.DTO.Dictionaries.Enterprises;

namespace GazRouter.DataServices.Bindings
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class BindingsService : ServiceBase, IBindingsService
    {
		#region EntitiesBindings

		public EntityBindingsPageDTO GetEntityBindingsPagedList(GetEntityBindingsPageParameterSet parameters)
		{
		    EntityBindingsPageDTO entityBindingsPageDTO = ExecuteRead<GetEntityBindingsPagedListQuery, EntityBindingsPageDTO, GetEntityBindingsPageParameterSet>(parameters);
		    return entityBindingsPageDTO;
		}


        public List<BindingDTO> GetEntityBindingsList(GetEntityBindingsPageParameterSet parameters)
        {
            var result = ExecuteRead<GetEntityBindingsListQuery, List<BindingDTO>, GetEntityBindingsPageParameterSet>(parameters);
            return result;
        }

        public Guid AddEntityBindings(EntityBindingParameterSet parameters)
		{
			return ExecuteRead<AddEntityBindingCommand, Guid, EntityBindingParameterSet>(parameters);
		}

		public void EditEntityBindings(EditEntityBindingParameterSet parameters)
		{
			ExecuteNonQuery<EditEntityBindingCommand, EditEntityBindingParameterSet>(parameters);
		}

        public void DeleteEntityBindings(Guid parameters)
		{
            ExecuteNonQuery<DeleteEntityBindingCommand, Guid>(parameters);
		}


        #endregion

		#region PropertyBindings

        public List<EnterpriseDTO> GetEnterpriseExchangeNeighbourList()
        {
            var currentEnterpriseId = AppSettingsManager.CurrentEnterpriseId;
            return ExecuteRead<GetEnterpriseExchangeNeighbourList, List<EnterpriseDTO>, Guid>(currentEnterpriseId);
        }

        public List<BindingDTO> GetPropertyBindingsList(GetPropertyBindingsParameterSet parameters)
		{
            return ExecuteRead<GetPropertyEntitiesPageQuery, List<BindingDTO>, GetPropertyBindingsParameterSet>(parameters);
		}

        public Guid AddPropertyBindings(AddPropertyBindingParameterSet parameters)
		{
			return ExecuteRead<AddPropertyBindingCommand, Guid, AddPropertyBindingParameterSet>(parameters);
		}

		public void EditPropertyBindings(EditPropertyBindingParameterSet parameters)
		{
			ExecuteNonQuery<EditPropertyBindingCommand, EditPropertyBindingParameterSet>(parameters);
		}

        public void DeletePropertyBindings(Guid parameters)
		{
            ExecuteNonQuery<DeletePropertyBindingCommand, Guid>(parameters);
		}

		#endregion

        #region EntitiesBindings

        public List<EntityPropertyBindingDTO> GetEntityPropertyBindingsList(GetEntityPropertyBindingsParameterSet parameters)
		{
			return ExecuteRead<GetEntityPropertyBindingPageQuery, List<EntityPropertyBindingDTO>, GetEntityPropertyBindingsParameterSet>(parameters);
		}

        public Guid AddEntityPropertyBindings(AddEntityPropertyBindingParameterSet parameters)
		{
			return ExecuteRead<AddEntityPropertyBindingCommand, Guid, AddEntityPropertyBindingParameterSet>(parameters);
		}

		public void EditEntityPropertyBindings(EditEntityPropertyBindingParameterSet parameters)
		{
			ExecuteNonQuery<EditEntityPropertyBindingCommand, EditEntityPropertyBindingParameterSet>(parameters);
		}

        public void DeleteEntityPropertyBindings(Guid parameters)
		{
            ExecuteNonQuery<DeleteEntityPropertyBindingCommand, Guid>(parameters);
		}

		#endregion
	}
}
