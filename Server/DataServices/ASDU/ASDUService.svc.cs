using System;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.ASDU;
using GazRouter.DAL.DispatcherTasks;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.ASDU;
using GazRouter.DAL.Core;
using Utils.Extensions;
using GazRouter.Service.Exchange.Lib;

namespace GazRouter.DataServices.ASDU
{
    [ErrorHandlerLogger("mainLogger")]
    //    [Authorization]
    public class ASDUService : ServiceBase, IASDUService
    {
        private static void SetLoggingProps(ManageRequestParams param)
        {
            //var rawGuid = ConfigurationManager.AppSettings["currentEnterpriseId"];
            //param.EnterpriseId = rawGuid.ToUpper(); // TODO!
            param.EnterpriseId =  AppSettingsManager.CurrentEnterpriseId.ToString("N").ToUpper();
            param.UserId = Session.User.Id.ToString();
        }

        private static string EnsureExchangeDir(LoadedFilesType filesType)
        {
            var path = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "ASDU_NSI");
            string endPoint;
            switch (filesType)
            {
                case LoadedFilesType.Input:
                    endPoint = "in";
                    break;
                case LoadedFilesType.Output:
                    endPoint = "out";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filesType));
            }
            return ExchangeHelper.EnsureDirectoryExist(path, endPoint);            
        }

        public List<AsduDataChange> GetImportLog(GetImportLogParam param)
        {
            using (var context = OpenDbContext())
            {
                var result = new GetImportLogEntriesCommand(context).Execute(param);
                return result;
            }
        }

        public string GetLoadedFileXml(LoadedFile param)
        {
            if (param.Status == LoadedFileStatus.InDir)
            {
                return File.ReadAllText(param.FileName);
            }
            using (var context = OpenDbContext())
            {
                var result = new GetLoadedFileXmlCommand(context).Execute(param.Key);
                return result;
            }
        }

        public List<LoadedFile> GetLoadedFiles(GetLoadedFilesParam param)
        {
            var result = new List<LoadedFile>();

            if (param.LoadedFilesType == LoadedFilesType.Input)
            {
                result.AddRange(Directory.EnumerateFiles(EnsureExchangeDir(LoadedFilesType.Input))
                    .Select(fileName => new LoadedFile
                    {
                        FileName = fileName,
                        Key = null,
                        Status = LoadedFileStatus.InDir,
                        LoadDate = File.GetCreationTime(fileName)
                    }));
            }
            using (var context = OpenDbContext())
            {
                result.AddRange(new GetLoadedFilesCommand(context).Execute(param));
            }

           
            return result;
        }

        public int ImportXmlFromMASDU(XmlFileForImport param)
        {
            if (param.LoadFromDisk)
            {
                param.Xml = File.ReadAllText(param.Filename);
            }
            using (var context = OpenDbContext())
            {
                var paramProxy = new ManageRequestParams
                {
                    Action = param.IsMetadataFile ? ManageRequestAction.LoadMeta : ManageRequestAction.Load,
                    Name = param.Filename,
                    Xml = param.Xml
                };

                SetLoggingProps(paramProxy);

                var result = new ManageAsduRequestCommand(context).Execute(paramProxy);

                if (param.LoadFromDisk)
                {
                    File.Delete(param.Filename);
                }
                return result;
            }
        }



        public List<DictionaryEntry> GetFilterParams(FilterType filterType)
        {
            using (var context = OpenDbContext())
            {
                var result = new GetFilterParamsCommand(context).Execute(filterType);
                return result;
            }
        }

        public List<MatchingTreeNode> GetTreeNodes(MatchingTreeNodeParams par)
        {
            using (var context = OpenDbContext())
            {
                var result = new GetTreeNodesCommand(context).Execute(par);
                return result;
            }
        }

        public List<DictionaryEntry> GetPossibleLinkRoles(LinkParams linkParams)
        {
            using (var context = OpenDbContext())
            {
                var result = new GetPossibleLinkRolesCommand(context).Execute(linkParams);
                return result;
            }
        }

        public void ManageLink(LinkParams linkParams)
        {
            using (var context = OpenDbContext())
            {
                new ManageLinkCommand(context).Execute(linkParams);
            }
        }

        public void AddAsduRequest(AsduRequestParams asduRequestParams)
        {
            using (var context = OpenDbContext())
            {
                new AddAsduRequestCommand(context).Execute(asduRequestParams);
            }
        }

        public void ManageAsduRequest(ManageRequestParams requestParams)
        {
            using (var context = OpenDbContext())
            {
                if (requestParams.Action == ManageRequestAction.Send)
                {
                    var xml = new GetLoadedFileXmlCommand(context).Execute(requestParams.Key);
                    File.WriteAllText(Path.Combine(EnsureExchangeDir(LoadedFilesType.Output), requestParams.Name), xml);
                }
                SetLoggingProps(requestParams);
                new ManageAsduRequestCommand(context).Execute(requestParams);
            }
        }

        public List<OutboundContents> GetAsduOutboundContents(int outboundKey)
        {
            using (var context = OpenDbContext())
            {
                var result = new GetAsduOutboundContentsCommand(context).Execute(outboundKey);
                return result;
            }
        }

        public void ManageAsduOutboundContents(ManageRequestParams requestParams)
        {
            using (var context = OpenDbContext())
            {
                new ManageAsduOutboundContentsCommand(context).Execute(requestParams);
            }
        }

        public string CreateAsduOutbound(int outboundPara)
        {
            using (var context = OpenDbContext())
            {
                var result = new CreateAsduOutboundCommand(context).Execute(outboundPara);
                return result;
            }
        }

        public void ApplyChangeToIus(MatchingTreeNode node)
        {
            using (var context = OpenDbContext())
            {
                new ApplyChangeToIusCommand(context).Execute(node);
            }
        }

        public IList<MetadataTreeNode> GetMetadataTreeNodes(MetadataTreeParams param)
        {
            using (var context = OpenDbContext())
            {
                var result = new GetMetadataTreeNodesCommand(context).Execute(param);
                return result;
            }
        }

        public List<NodeBinding> GetPossibleDataTreeRoots()
        {
            using (var context = OpenDbContext())
            {
                var result = new GetPossibleDataTreeRootsCommand(context).Execute();
                return result;
            }
        }

        public IList<DataTreeNode> GetDataTreeNodes(DataTreeParams param)
        {
            using (var context = OpenDbContext())
            {
                var result = new GetDataTreeNodesCommand(context).Execute(param);
                return result;
            }
        }
    }
}