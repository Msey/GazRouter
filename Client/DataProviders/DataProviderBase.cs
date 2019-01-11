using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using DataProviders;
using GazRouter.Common;

namespace GazRouter.DataProviders
{
    public abstract class DataProviderBase<TService> where TService : class
    {
        private static ChannelFactory<TService> _channelFactory;

        protected abstract string ServiceUri { get; }

        protected TService GetChannel()
        {
            if ((_channelFactory == null) || (_channelFactory.State == CommunicationState.Faulted))
            {
                var binding =
                    new BasicHttpBinding(DataProvideSettings.ServerUri.Scheme == "https"
                        ? BasicHttpSecurityMode.Transport
                        : BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 2147483647,
                        MaxBufferSize = 2147483647,
                        SendTimeout = TimeSpan.FromMinutes(5),
                        ReceiveTimeout = TimeSpan.FromMinutes(5),
                        OpenTimeout = TimeSpan.FromMinutes(1),
                        CloseTimeout = TimeSpan.FromMinutes(1)
                    };

                var versionHeader = AddressHeader.CreateAddressHeader("ClientVersion", string.Empty,
                    DataProvideSettings.ClientVersion);

                var endpointAddress = new EndpointAddress(UriBuilder.GetServiceUri(ServiceUri), versionHeader);

                _channelFactory = new ChannelFactory<TService>(binding, endpointAddress);
            }

            return _channelFactory.CreateChannel();
        }

        protected async Task<TResult> ExecuteAsync<TResult, TParam>(TService channel,
            Func<TParam, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod,
            TParam parameters)
        {
            TResult result;
            try
            {
                result = await Task<TResult>.Factory.FromAsync(beginMethod, endMethod, parameters, null);
                ((IClientChannel) channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel) channel).Abort();
                throw new ServerException(ex);
            }
            return result;
        }

        protected async Task<TResult> ExecuteAsync<TResult>(TService channel,
            Func<object, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod)
        {
            TResult result;
            try
            {
                result = await Task<TResult>.Factory.FromAsync(beginMethod, endMethod, null, null);
                ((IClientChannel) channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel) channel).Abort();
                throw new ServerException(ex);
            }
            return result;
        }

        protected async Task ExecuteAsync<TParam>(TService channel,
            Func<TParam, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod,
            TParam parameters)
        {
            try
            {
                await Task.Factory.FromAsync(beginMethod, endMethod, parameters, null);
                ((IClientChannel) channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel) channel).Abort();
                throw new ServerException(ex);
            }
        }

        protected async Task ExecuteAsync(TService channel,
            Func<object, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod)
        {
            try
            {
                await Task.Factory.FromAsync(beginMethod, endMethod, null, null);
                ((IClientChannel) channel).Close();
            }
            catch (Exception ex)
            {
                ((IClientChannel) channel).Abort();
                throw new ServerException(ex);
            }
        }
    }
}