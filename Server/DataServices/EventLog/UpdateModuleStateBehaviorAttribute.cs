using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO;

namespace GazRouter.DataServices.EventLog
{
    public class UpdateModuleStateBehaviorAttribute : Attribute, IOperationBehavior
    {
        public Module Module { get; set; }

        public UpdateModuleStateBehaviorAttribute(Module module)
        {
            Module = module;
        }

        public void Validate(OperationDescription operationDescription)
        {
            
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            if (operationDescription == null) throw new ArgumentNullException("operationDescription");
            if (dispatchOperation == null) throw new ArgumentNullException("dispatchOperation");

            dispatchOperation.Invoker = new UpdateModuleStateInvoker(dispatchOperation.Invoker, Module);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {

        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
          
        }
    }

    public class UpdateModuleStateInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker _originalInvoker;
        private readonly Module _module;

        public UpdateModuleStateInvoker(IOperationInvoker originalInvoker, Module module)
        {
            _originalInvoker = originalInvoker;
            _module = module;
        }

        public object[] AllocateInputs()
        {
            return _originalInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var operationalResult = _originalInvoker.Invoke(instance, inputs, out outputs);
            SetNeedRefresh(); 
            return operationalResult;
        }

        private void SetNeedRefresh()
        {
            ModelState.ChangeState(_module);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return _originalInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            var operationalResult = _originalInvoker.InvokeEnd(instance, out outputs, result);
            SetNeedRefresh();
            return operationalResult;
        }

        public bool IsSynchronous { get { return _originalInvoker.IsSynchronous; } }
    }
}