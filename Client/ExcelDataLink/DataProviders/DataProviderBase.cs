using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Threading.Tasks;

namespace DataProviders
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
        }
    }
    public abstract class DataProviderBase<TService> where TService : class
    {
        private static ChannelFactory<TService> _channelFactory;

        protected DataProviderBase()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
        }

    protected TService GetChannel()
    {
        if ((_channelFactory == null) || (_channelFactory.State == CommunicationState.Faulted))
        {
            var bindingsSection =
                ConfigurationManager.GetSection("system.serviceModel/bindings") as
                    BindingsSection;          
            Debug.Assert(bindingsSection != null, "bindingConfig != null");

            var bindingConfig = bindingsSection.BasicHttpBinding.Bindings[0];

            var binding = new BasicHttpBinding()
            {
                MaxReceivedMessageSize = bindingConfig.MaxReceivedMessageSize,
                MaxBufferSize = bindingConfig.MaxBufferSize,
                SendTimeout = bindingConfig.SendTimeout,
                ReceiveTimeout = bindingConfig.ReceiveTimeout,
                OpenTimeout = bindingConfig.OpenTimeout,
                CloseTimeout = bindingConfig.CloseTimeout,
                Security = new BasicHttpSecurity { Mode = bindingConfig.Security.Mode, Transport = new HttpTransportSecurity { ClientCredentialType = bindingConfig.Security.Transport.ClientCredentialType} }
            };

            var clientConfig = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
            Debug.Assert(clientConfig != null, "clientConfig != null");

            var endpointElements = clientConfig.Endpoints.OfType<ChannelEndpointElement>().ToList();
            var endpointElement = endpointElements.FirstOrDefault(ep => ep.BindingConfiguration == bindingConfig.Name);
            Debug.Assert(endpointElement != null, "endpointElement != null");

            var uri = endpointElement.Address.Append(ServiceUri);
            var versionHeader = AddressHeader.CreateAddressHeader("ClientVersion", string.Empty, DataProvideSettings.ClientVersion);
            var endpointAddress = new EndpointAddress(uri, versionHeader);
            _channelFactory = new ChannelFactory<TService>(binding, endpointAddress);
        }

        return _channelFactory.CreateChannel();
    }

        protected abstract string ServiceUri { get; }

        protected void Execute<TParameters, TResult>(TService channel, Func<TParameters, AsyncCallback, object, IAsyncResult> beginOp, Func<IAsyncResult, TResult> endOp, Func<TResult, Exception, bool> callback, TParameters parameters, IClientBehavior behavior)
        {
            var wrapper = new AsyncFuncWrapper<TParameters, TResult>(channel, beginOp, endOp, callback, behavior);
            wrapper.Invoke(parameters);
        }

        protected void Execute<TResult>(TService channel, Func<object, AsyncCallback, object, IAsyncResult> beginOp, Func<IAsyncResult, TResult> endOp, Func<TResult, Exception, bool> callback, IClientBehavior behavior)
        {
            var wrapper = new AsyncFuncWrapper<object, TResult>(channel, beginOp, endOp, callback, behavior);
            wrapper.Invoke();
        }

        protected void Execute<TParameters>(TService channel, Func<TParameters, AsyncCallback, object, IAsyncResult> beginOp, Action<IAsyncResult> endOp, Func<Exception, bool> callback, TParameters parameters, IClientBehavior behavior)
        {
            var wrapper = new AsyncActionWrapper<TParameters>(channel, beginOp, endOp, callback, behavior);
            wrapper.Invoke(parameters);
        }

        protected void Execute(TService channel, Func<object, AsyncCallback, object, IAsyncResult> beginOp, Action<IAsyncResult> endOp, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var wrapper = new AsyncActionWrapper<object>(channel, beginOp, endOp, callback, behavior);
            wrapper.Invoke();
        }

        private abstract class AsyncWrapper<TParameters>
        {
            private readonly Func<TParameters, AsyncCallback, object, IAsyncResult> _beginOp;
            private readonly object _channel;
            private readonly IClientBehavior _behavior;

            protected AsyncWrapper(object channel,
                Func<TParameters, AsyncCallback, object, IAsyncResult> beginOp, IClientBehavior behavior)
            {
                _beginOp = beginOp;
                _channel = channel;
                _behavior = behavior;
            }

            public void Invoke(TParameters parameters = default(TParameters))
            {
                _behavior.OnBeforeQuery();

                _beginOp(parameters, AsyncCallbackMethod, null);
            }

            private void AsyncCallbackMethod(IAsyncResult asyncResult)
            {
                Exception exception = null;
                try
                {
                    CallEndOp(asyncResult);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                _behavior.ProcessResult(exception, CallUICallback);

                ((IDisposable)_channel).Dispose();
            }

            protected abstract bool CallUICallback(Exception exception);

            protected abstract void CallEndOp(IAsyncResult asyncResult);
        }

        private class AsyncFuncWrapper<TParameters, TResult> : AsyncWrapper<TParameters>
        {
            private readonly Func<IAsyncResult, TResult> _endOp;
            private readonly Func<TResult, Exception, bool> _callback;
            private TResult _result;

            public AsyncFuncWrapper(object channel,
                Func<TParameters, AsyncCallback, object, IAsyncResult> beginOp,
                Func<IAsyncResult, TResult> endOp,
                Func<TResult, Exception, bool> callback, IClientBehavior behavior)
                : base(channel, beginOp, behavior)
            {
                _endOp = endOp;
                _callback = callback;
            }

            protected override bool CallUICallback(Exception exception)
            {
                return _callback(_result, exception);
            }

            protected override void CallEndOp(IAsyncResult asyncResult)
            {
                _result = _endOp(asyncResult);
            }
        }


        private class AsyncActionWrapper<TParameters> : AsyncWrapper<TParameters>
        {
            private readonly Action<IAsyncResult> _endOp;
            private readonly Func<Exception, bool> _callback;

            public AsyncActionWrapper(object channel,
                Func<TParameters, AsyncCallback, object, IAsyncResult> beginOp,
                Action<IAsyncResult> endOp,
                Func<Exception, bool> callback, IClientBehavior behavior)
                : base(channel, beginOp, behavior)
            {
                _endOp = endOp;
                _callback = callback;
            }

            protected override bool CallUICallback(Exception exception)
            {
                return _callback(exception);
            }

            protected override void CallEndOp(IAsyncResult asyncResult)
            {
                _endOp(asyncResult);
            }
        }

        protected async Task<TResult> ExecuteAsync<TResult, TParam>(TService channel, Func<TParam, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TParam parameters)
        {
            TResult result;
            try
            {
                result = await Task<TResult>.Factory.FromAsync(beginMethod, endMethod, parameters, null);
                ((IClientChannel)channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel)channel).Abort();
                throw new ServerException(ex);
            }
            return result;
        }

        protected async Task<TResult> ExecuteAsync<TResult>(TService channel, Func<object, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod)
        {
            TResult result;
            try
            {
                result = await Task<TResult>.Factory.FromAsync(beginMethod, endMethod, null, null);
                ((IClientChannel)channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel)channel).Abort();
                throw new ServerException(ex);
            }
            return result;
        }

        protected async Task ExecuteAsync<TParam>(TService channel, Func<TParam, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TParam parameters)
        {
            try
            {
                await Task.Factory.FromAsync(beginMethod, endMethod, parameters, null);
                ((IClientChannel)channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel)channel).Abort();
                throw new ServerException(ex);
            }
        }

        protected async Task ExecuteAsync(TService channel, Func<object, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod)
        {
            try
            {
                await Task.Factory.FromAsync(beginMethod, endMethod, null, null);
                ((IClientChannel)channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel)channel).Abort();
                throw new ServerException(ex);
            }
        }

    }
}
