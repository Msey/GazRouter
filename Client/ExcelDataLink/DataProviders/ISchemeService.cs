using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.Appearance.Versions;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.ObjectModel  
{
    [ServiceContract]
    public interface ISchemeService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddScheme(SchemeParameterSet parameters, AsyncCallback callback, object state);
        int EndAddScheme(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSchemeVersionList(object parameters, AsyncCallback callback, object state);
        List<SchemeVersionItemDTO> EndGetSchemeVersionList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPublishedSchemeVersionList(object parameters, AsyncCallback callback, object state);
        List<SchemeVersionItemDTO> EndGetPublishedSchemeVersionList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSchemeVersionById(int parameters, AsyncCallback callback, object state);
        SchemeVersionDTO EndGetSchemeVersionById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddSchemeVersion(SchemeVersionParameterSet parameters, AsyncCallback callback, object state);
        int EndAddSchemeVersion(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteSchemeVersion(int parameters, AsyncCallback callback, object state);
        void EndDeleteSchemeVersion(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginPublishSchemeVersion(int parameters, AsyncCallback callback, object state);
        void EndPublishSchemeVersion(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUnPublishSchemeVersion(int parameters, AsyncCallback callback, object state);
        void EndUnPublishSchemeVersion(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetFullSchemeModel(int parameters, AsyncCallback callback, object state);
        SchemeModelDTO EndGetFullSchemeModel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginCommentSchemeVersion(CommentSchemeVersionParameterSet parameters, AsyncCallback callback, object state);
        void EndCommentSchemeVersion(IAsyncResult result);
    }


    public class SchemeServiceProxy : DataProviderBase<ISchemeService>
	{
        protected override string ServiceUri
        {
            get { return "/ObjectModel/SchemeService.svc"; }
        }

        public Task<int> AddSchemeAsync(SchemeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,SchemeParameterSet>(channel, channel.BeginAddScheme, channel.EndAddScheme, parameters);
        }

        public Task<List<SchemeVersionItemDTO>> GetSchemeVersionListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SchemeVersionItemDTO>>(channel, channel.BeginGetSchemeVersionList, channel.EndGetSchemeVersionList);
        }

        public Task<List<SchemeVersionItemDTO>> GetPublishedSchemeVersionListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SchemeVersionItemDTO>>(channel, channel.BeginGetPublishedSchemeVersionList, channel.EndGetPublishedSchemeVersionList);
        }

        public Task<SchemeVersionDTO> GetSchemeVersionByIdAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<SchemeVersionDTO,int>(channel, channel.BeginGetSchemeVersionById, channel.EndGetSchemeVersionById, parameters);
        }

        public Task<int> AddSchemeVersionAsync(SchemeVersionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,SchemeVersionParameterSet>(channel, channel.BeginAddSchemeVersion, channel.EndAddSchemeVersion, parameters);
        }

        public Task DeleteSchemeVersionAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteSchemeVersion, channel.EndDeleteSchemeVersion, parameters);
        }

        public Task PublishSchemeVersionAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginPublishSchemeVersion, channel.EndPublishSchemeVersion, parameters);
        }

        public Task UnPublishSchemeVersionAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUnPublishSchemeVersion, channel.EndUnPublishSchemeVersion, parameters);
        }

        public Task<SchemeModelDTO> GetFullSchemeModelAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<SchemeModelDTO,int>(channel, channel.BeginGetFullSchemeModel, channel.EndGetFullSchemeModel, parameters);
        }

        public Task CommentSchemeVersionAsync(CommentSchemeVersionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginCommentSchemeVersion, channel.EndCommentSchemeVersion, parameters);
        }

    }
}
