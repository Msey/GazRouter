using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using GazRouter.DTO.ASDU;
using System.Collections.Generic;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.ASDU  
{
    [ServiceContract]
    public interface IASDUService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetLoadedFiles(GetLoadedFilesParam param, AsyncCallback callback, object state);
        List<LoadedFile> EndGetLoadedFiles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginImportXmlFromMASDU(XmlFileForImport param, AsyncCallback callback, object state);
        int EndImportXmlFromMASDU(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetImportLog(GetImportLogParam param, AsyncCallback callback, object state);
        List<AsduDataChange> EndGetImportLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetLoadedFileXml(LoadedFile param, AsyncCallback callback, object state);
        string EndGetLoadedFileXml(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetFilterParams(FilterType filterType, AsyncCallback callback, object state);
        List<DictionaryEntry> EndGetFilterParams(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTreeNodes(MatchingTreeNodeParams par, AsyncCallback callback, object state);
        List<MatchingTreeNode> EndGetTreeNodes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPossibleLinkRoles(LinkParams linkParams, AsyncCallback callback, object state);
        List<DictionaryEntry> EndGetPossibleLinkRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginManageLink(LinkParams linkParams, AsyncCallback callback, object state);
        void EndManageLink(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddAsduRequest(AsduRequestParams asduRequestParams, AsyncCallback callback, object state);
        void EndAddAsduRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginManageAsduRequest(ManageRequestParams requestParams, AsyncCallback callback, object state);
        void EndManageAsduRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAsduOutboundContents(int outboundPara, AsyncCallback callback, object state);
        List<OutboundContents> EndGetAsduOutboundContents(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginManageAsduOutboundContents(ManageRequestParams requestParams, AsyncCallback callback, object state);
        void EndManageAsduOutboundContents(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginCreateAsduOutbound(int outboundPara, AsyncCallback callback, object state);
        string EndCreateAsduOutbound(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginApplyChangeToIus(MatchingTreeNode node, AsyncCallback callback, object state);
        void EndApplyChangeToIus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetMetadataTreeNodes(MetadataTreeParams param, AsyncCallback callback, object state);
        IList<MetadataTreeNode> EndGetMetadataTreeNodes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPossibleDataTreeRoots(object parameters, AsyncCallback callback, object state);
        List<NodeBinding> EndGetPossibleDataTreeRoots(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDataTreeNodes(DataTreeParams param, AsyncCallback callback, object state);
        IList<DataTreeNode> EndGetDataTreeNodes(IAsyncResult result);
    }

	public interface IASDUServiceProxy
	{

        Task<List<LoadedFile>> GetLoadedFilesAsync(GetLoadedFilesParam param);

        Task<int> ImportXmlFromMASDUAsync(XmlFileForImport param);

        Task<List<AsduDataChange>> GetImportLogAsync(GetImportLogParam param);

        Task<string> GetLoadedFileXmlAsync(LoadedFile param);

        Task<List<DictionaryEntry>> GetFilterParamsAsync(FilterType filterType);

        Task<List<MatchingTreeNode>> GetTreeNodesAsync(MatchingTreeNodeParams par);

        Task<List<DictionaryEntry>> GetPossibleLinkRolesAsync(LinkParams linkParams);

        Task ManageLinkAsync(LinkParams linkParams);

        Task AddAsduRequestAsync(AsduRequestParams asduRequestParams);

        Task ManageAsduRequestAsync(ManageRequestParams requestParams);

        Task<List<OutboundContents>> GetAsduOutboundContentsAsync(int outboundPara);

        Task ManageAsduOutboundContentsAsync(ManageRequestParams requestParams);

        Task<string> CreateAsduOutboundAsync(int outboundPara);

        Task ApplyChangeToIusAsync(MatchingTreeNode node);

        Task<IList<MetadataTreeNode>> GetMetadataTreeNodesAsync(MetadataTreeParams param);

        Task<List<NodeBinding>> GetPossibleDataTreeRootsAsync();

        Task<IList<DataTreeNode>> GetDataTreeNodesAsync(DataTreeParams param);

    }

    public sealed class ASDUServiceProxy : DataProviderBase<IASDUService>, IASDUServiceProxy
	{
        protected override string ServiceUri => "/ASDU/ASDUService.svc";
      


        public Task<List<LoadedFile>> GetLoadedFilesAsync(GetLoadedFilesParam param)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<LoadedFile>,GetLoadedFilesParam>(channel, channel.BeginGetLoadedFiles, channel.EndGetLoadedFiles, param);
        }

        public Task<int> ImportXmlFromMASDUAsync(XmlFileForImport param)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,XmlFileForImport>(channel, channel.BeginImportXmlFromMASDU, channel.EndImportXmlFromMASDU, param);
        }

        public Task<List<AsduDataChange>> GetImportLogAsync(GetImportLogParam param)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AsduDataChange>,GetImportLogParam>(channel, channel.BeginGetImportLog, channel.EndGetImportLog, param);
        }

        public Task<string> GetLoadedFileXmlAsync(LoadedFile param)
        {
            var channel = GetChannel();
            return ExecuteAsync<string,LoadedFile>(channel, channel.BeginGetLoadedFileXml, channel.EndGetLoadedFileXml, param);
        }

        public Task<List<DictionaryEntry>> GetFilterParamsAsync(FilterType filterType)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DictionaryEntry>,FilterType>(channel, channel.BeginGetFilterParams, channel.EndGetFilterParams, filterType);
        }

        public Task<List<MatchingTreeNode>> GetTreeNodesAsync(MatchingTreeNodeParams par)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<MatchingTreeNode>,MatchingTreeNodeParams>(channel, channel.BeginGetTreeNodes, channel.EndGetTreeNodes, par);
        }

        public Task<List<DictionaryEntry>> GetPossibleLinkRolesAsync(LinkParams linkParams)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DictionaryEntry>,LinkParams>(channel, channel.BeginGetPossibleLinkRoles, channel.EndGetPossibleLinkRoles, linkParams);
        }

        public Task ManageLinkAsync(LinkParams linkParams)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginManageLink, channel.EndManageLink, linkParams);
        }

        public Task AddAsduRequestAsync(AsduRequestParams asduRequestParams)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddAsduRequest, channel.EndAddAsduRequest, asduRequestParams);
        }

        public Task ManageAsduRequestAsync(ManageRequestParams requestParams)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginManageAsduRequest, channel.EndManageAsduRequest, requestParams);
        }

        public Task<List<OutboundContents>> GetAsduOutboundContentsAsync(int outboundPara)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<OutboundContents>,int>(channel, channel.BeginGetAsduOutboundContents, channel.EndGetAsduOutboundContents, outboundPara);
        }

        public Task ManageAsduOutboundContentsAsync(ManageRequestParams requestParams)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginManageAsduOutboundContents, channel.EndManageAsduOutboundContents, requestParams);
        }

        public Task<string> CreateAsduOutboundAsync(int outboundPara)
        {
            var channel = GetChannel();
            return ExecuteAsync<string,int>(channel, channel.BeginCreateAsduOutbound, channel.EndCreateAsduOutbound, outboundPara);
        }

        public Task ApplyChangeToIusAsync(MatchingTreeNode node)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginApplyChangeToIus, channel.EndApplyChangeToIus, node);
        }

        public Task<IList<MetadataTreeNode>> GetMetadataTreeNodesAsync(MetadataTreeParams param)
        {
            var channel = GetChannel();
            return ExecuteAsync<IList<MetadataTreeNode>,MetadataTreeParams>(channel, channel.BeginGetMetadataTreeNodes, channel.EndGetMetadataTreeNodes, param);
        }

        public Task<List<NodeBinding>> GetPossibleDataTreeRootsAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<NodeBinding>>(channel, channel.BeginGetPossibleDataTreeRoots, channel.EndGetPossibleDataTreeRoots);
        }

        public Task<IList<DataTreeNode>> GetDataTreeNodesAsync(DataTreeParams param)
        {
            var channel = GetChannel();
            return ExecuteAsync<IList<DataTreeNode>,DataTreeParams>(channel, channel.BeginGetDataTreeNodes, channel.EndGetDataTreeNodes, param);
        }

    }
}
