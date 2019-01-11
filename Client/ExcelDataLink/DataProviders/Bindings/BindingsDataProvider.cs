using System;
using System.Collections.Generic;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.EntityPropertyBindings;

namespace DataProviders.Bindings
{
    public class BindingsDataProvider : DataProviderBase<IBindingsService>
	{
        protected override string ServiceUri
        {
            get { return "/Bindings/BindingsService.svc"; }

        }
        
		#region EntityBindings

		public void GetEntityBindingsList(GetEntityBindingsPageParameterSet parameters, Func<List<BindingDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetEntityBindingsList, channel.EndGetEntityBindingsList, callback,parameters, behavior);
		}

        public void DeleteEntityBindings(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginDeleteEntityBindings, channel.EndDeleteEntityBindings, callback, parameters, behavior);
		}

        public void AddEntityBindings(EntityBindingParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginAddEntityBindings, channel.EndAddEntityBindings, callback, parameters, behavior);
		}

		public void EditEntityBindings(EditEntityBindingParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginEditEntityBindings, channel.EndEditEntityBindings, callback, parameters, behavior);
		}

		#endregion

        #region EntityBindingsProperty

        public void GetEntityPropertyBindingsList(GetEntityPropertyBindingsParameterSet parameters, Func<List<EntityPropertyBindingDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetEntityPropertyBindingsList, channel.EndGetEntityPropertyBindingsList, callback, parameters, behavior);
		}

        public void DeleteEntityPropertyBindings(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginDeleteEntityPropertyBindings, channel.EndDeleteEntityPropertyBindings, callback, parameters, behavior);
		}

        public void AddEntityPropertyBindings(AddEntityPropertyBindingParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginAddEntityPropertyBindings, channel.EndAddEntityPropertyBindings, callback, parameters, behavior);
		}

		public void EditEntityPropertyBindings(EditEntityPropertyBindingParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginEditEntityPropertyBindings, channel.EndEditEntityPropertyBindings, callback, parameters, behavior);
		}

		#endregion

	}
}
