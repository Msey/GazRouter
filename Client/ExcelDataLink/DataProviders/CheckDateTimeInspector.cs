using System;
using System.Collections;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DataProviders
{
    public class CheckDateTimeInspectorBehavior : IOperationBehavior
    {
        public void Validate(OperationDescription operationDescription)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            clientOperation.ParameterInspectors.Add(new CheckDateTimeInspector());
        }

        public void AddBindingParameters(OperationDescription operationDescription,
                                         BindingParameterCollection bindingParameters)
        {
        }
    }


    public class CheckDateTimeInspector : IParameterInspector
    {
        public object BeforeCall(string operationName, object[] inputs)
        {
            foreach (object input in inputs)
            {
                CheckDateTime(input, 0);
            }
            return inputs;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }

        private void CheckDateTime(object o, int deep)
        {
            if (deep >= 10)
                return;
            if (o == null)
                return;

            if (o is string)
                return;

            if (o is IEnumerable)
            {
                foreach (object v in o as IEnumerable)
                {
                    CheckDateTime(v, deep + 1);
                }
                return;
            }

            Type t = o.GetType();

            if (t == typeof (DateTime))
            {
                var value = (DateTime) o;
                if (value.Kind != DateTimeKind.Local)
                    throw new Exception("Datetime.Kind must be Local");
                return;
            }

            if (t.IsEnum | t.IsValueType)
                return;


            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (!pi.CanRead)
                    continue;

                if (pi.GetIndexParameters().Length > 0)
                    continue;


                CheckDateTime(pi.GetValue(o, null), deep + 1);
            }
        }
    }
}