using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.Appearance.Versions;

namespace GazRouter.DataServices.ObjectModel
{
    [Service("Схема")]
    [ServiceContract]
    public interface ISchemeService
    {
        [ServiceAction("Добавление схемы")]
        [OperationContract]
        int AddScheme(SchemeParameterSet parameters);

        [ServiceAction("Получение версий схем")]
        [OperationContract]
        List<SchemeVersionItemDTO> GetSchemeVersionList();

        [ServiceAction("Получение последних опубликованных версий схем")]
        [OperationContract]
        List<SchemeVersionItemDTO> GetPublishedSchemeVersionList();

        [ServiceAction("Получение версии схемы по id")]
        [OperationContract]
        SchemeVersionDTO GetSchemeVersionById(int parameters);

        [ServiceAction("Добавление версии схемы")]
        [OperationContract]
        int AddSchemeVersion(SchemeVersionParameterSet parameters);

        [ServiceAction("Удаление версии схемы")]
        [OperationContract]
        void DeleteSchemeVersion(int parameters);

        [ServiceAction("Публикация версии схемы")]
        [OperationContract]
        void PublishSchemeVersion(int parameters);

        [ServiceAction("Отмена публикации версии схемы")]
        [OperationContract]
        void UnPublishSchemeVersion(int parameters);

        [ServiceAction("Получение списка данных для схемы")]
        [OperationContract]
        SchemeModelDTO GetFullSchemeModel(int parameters);

        [ServiceAction("Добавление комментария к версии схемы")]
        [OperationContract]
        void CommentSchemeVersion(CommentSchemeVersionParameterSet parameters);
    }
}
