using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.EntityPropertyBindings;

namespace GazRouter.DataServices.Bindings
{
    [Service("Информационный обмен")]
    [ServiceContract]
    public interface IBindingsService
    {
        #region EntityBindings

		[ServiceAction("Получение списка привязок")]
		[OperationContract]
		List<BindingDTO> GetEntityBindingsList(GetEntityBindingsPageParameterSet parameters);

		[ServiceAction("Добавление привязки")]
		[OperationContract]
        Guid AddEntityBindings(EntityBindingParameterSet parameters);

		[ServiceAction("Редактирование привязки")]
		[OperationContract]
		void EditEntityBindings(EditEntityBindingParameterSet parameters);

		[ServiceAction("Удаление привязки")]
		[OperationContract]
		void DeleteEntityBindings(Guid parameters);

		#endregion

        #region EntityPropertyBindings

        [ServiceAction("Получение списка привязок свойств")]
        [OperationContract]
        List<EntityPropertyBindingDTO> GetEntityPropertyBindingsList(GetEntityPropertyBindingsParameterSet parameters);

        [ServiceAction("Добавление привязки свойсва")]
        [OperationContract]
        Guid AddEntityPropertyBindings(AddEntityPropertyBindingParameterSet parameters);

        [ServiceAction("Редактирование привязки свойтва")]
        [OperationContract]
        void EditEntityPropertyBindings(EditEntityPropertyBindingParameterSet parameters);

        [ServiceAction("Удаление привязки свойства")]
        [OperationContract]
        void DeleteEntityPropertyBindings(Guid parameters);

        #endregion
    }
}
