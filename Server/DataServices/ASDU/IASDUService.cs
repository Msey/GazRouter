using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.ASDU;
using System.Collections.Generic;

namespace GazRouter.DataServices.ASDU
{
    [Service("Поставщик для интеграции c АСДУ")]
    [ServiceContract]
    public interface IASDUService
    {
        [ServiceAction("Получение списка файлов заявок", true)]
        [OperationContract]
        List<LoadedFile> GetLoadedFiles(GetLoadedFilesParam param);

        [ServiceAction("Импорт файла xml", true)]
        [OperationContract]
        int ImportXmlFromMASDU(XmlFileForImport param);

        [ServiceAction("Получение содержимого заявки", true)]
        [OperationContract]
        List<AsduDataChange> GetImportLog(GetImportLogParam param);

        [ServiceAction("Получение содержимого файла в виде XML", true)]
        [OperationContract]
        string GetLoadedFileXml(LoadedFile param);

        [ServiceAction("Получение параметров для выпадающих списков и фильтров", true)]
        [OperationContract]
        List<DictionaryEntry> GetFilterParams(FilterType filterType);

        [ServiceAction("Получение узлов дерева", true)]
        [OperationContract]
        List<MatchingTreeNode> GetTreeNodes(MatchingTreeNodeParams par);

        [ServiceAction("Получение возможных типов связывания", true)]
        [OperationContract]
        List<DictionaryEntry> GetPossibleLinkRoles(LinkParams linkParams);

        [ServiceAction("Связать / разорвать связь между элементами деревьев", true)]
        [OperationContract]
        void ManageLink(LinkParams linkParams);

        [ServiceAction("Добавить заявку на изменение", true)]
        [OperationContract]
        void AddAsduRequest(AsduRequestParams asduRequestParams);

        [ServiceAction("Управление заявкой", true)]
        [OperationContract]
        void ManageAsduRequest(ManageRequestParams requestParams);

        [ServiceAction("Получение содержимого заявки", true)]
        [OperationContract]
        List<OutboundContents> GetAsduOutboundContents(int outboundPara);

        [ServiceAction("Управление содержимым заявки", true)]
        [OperationContract]
        void ManageAsduOutboundContents(ManageRequestParams requestParams);

        [ServiceAction("Формирование заявки в виде XML", true)]
        [OperationContract]
        string CreateAsduOutbound(int outboundPara);

        [ServiceAction("Применить изменения в ИУС", true)]
        [OperationContract]
        void ApplyChangeToIus(MatchingTreeNode node);

        [ServiceAction("Получить узлы дерева метаданных", true)]
        [OperationContract]
        IList<MetadataTreeNode> GetMetadataTreeNodes(MetadataTreeParams param);


        [ServiceAction("Получение возможных типов корневых объектов для деревьев данных", true)]
        [OperationContract]
        List<NodeBinding> GetPossibleDataTreeRoots();

        [ServiceAction("Получить узлы дерева данных", true)]
        [OperationContract]
        IList<DataTreeNode> GetDataTreeNodes(DataTreeParams param);
    }
}
