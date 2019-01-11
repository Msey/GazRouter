using Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.DataExchange.Transformation;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Import;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DAL.DataExchange.Integro;
//using Integro.Interfaces;
//using Integro.IUSExchange.Export;
using System.IO;
using System.Globalization;
//using Integro.Common;
//using Integro.Interfaces.Classes;
//using Integro.Interfaces.Enums;
using System.Configuration;
using Integro.Interfaces;
using Integro.Interfaces.Classes;
using Integro.Interfaces.Enums;
using GazRouter.DataServices.Infrastructure;
using GazRouter.Service.Exchange.Lib.AsduEsg;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel;
using GazRouter.DAL.DataExchange.Integro.SummaryExchangeTasks;

namespace GazRouter.DataServices.Integro
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class IntegroService : ServiceBase, IIntegroService
    {
        /// <summary>
        /// Экспортсводки
        /// </summary>
        /// <param name="parameters"> Параметры экспорта</param>
        /// <returns>результат экспорта</returns>
        public ExportResult ExportSummary(ExportSummaryParams parameters)
        {
            var exporter = new AsduEsgExchangeObjectExporter();
            return exporter.ExportSummary(parameters);

        }
        /// <summary>
        /// Сохранение сводки с созданием таски на обмен
        /// </summary>
        /// <param name="param">Данные для сохранения сводки и таски </param>
        public Guid SaveSummaryExchTask(SummaryExchTaskParamSet param)
        {
            using (var context = OpenDbContext())
            {
                var manager = new SummaryExchangeTasksManager(context);
                var result = manager.SaveSummaryExchangeTask(param);
                return result;
            }
        }
        public List<SummaryDTO> GetSummariesList()
        {
            return ExecuteRead<GetSummariesListQuery, List<SummaryDTO>>();
        }

        public List<SummaryDTO> GetSummariesListByParams(GetSummaryParameterSet parameter)
        {
            return ExecuteRead<GetSummariesListByParamsIdQuery, List<SummaryDTO>, GetSummaryParameterSet>(parameter);
        }

        public void AddEditSummary(AddEditSummaryParameterSet parameters)
        {
            ExecuteNonQuery<AddEditSummaryCommand, AddEditSummaryParameterSet>(parameters);
        }

        public void DeleteSummary(Guid parameters)
        {
            ExecuteNonQuery<DeleteSummaryCommand, Guid>(parameters);
        }

        public void DeleteSummaryParam(Guid parameters)
        {
            ExecuteNonQuery<DeleteSummaryParamCommand, Guid>(parameters);
        }

        public void AddEditSummaryParam(AddEditSummaryPParameterSet parameters)
        {
            ExecuteNonQuery<AddEditSummaryParamCommand, AddEditSummaryPParameterSet>(parameters);
            
            foreach (var item in parameters.ContentList)
            {
                AddEditSummaryParamContent(item);
            }
        }

        public SummatyLoadResult AddSummaryParamList(List<AddEditSummaryPParameterSet> parameters)
        {
            var result = new SummatyLoadResult();
            if (parameters == null || !parameters.Any())
                return result;
            result.SummaryId = parameters.First().SummaryId;
            foreach (var summary in parameters)
            {
                var entGuid = summary.ContentList.First().EntityId;
                var ent = ExecuteRead<CheckEntityQuery, bool, Guid>(entGuid);
                if (!ent)
                {
                    result.NotFoundEntity.Add(summary);
                    continue;
                }
                AddEditSummaryParam(summary);
                result.LoadedEntity.Add(summary);
            }
            return result;
        }

        public void AddEditSummaryParamContent(AddEditSummaryPContentParameterSet parameters)
        {
            ExecuteNonQuery<AddEditSummaryParamContentCommand, AddEditSummaryPContentParameterSet>(parameters);
        }

        public List<SummaryParamDTO> GetSummariesParamList(Guid parameter)
        {
            var result = ExecuteRead<GetSummariesParamListQuery, List<SummaryParamDTO>, Guid>(parameter);
            foreach(var item in result)
            {
                item.SummaryParamContentList = GetSummariesParamContentList(item.Id);
            }
            return result;
            
        }

        public List<SummaryParamContentDTO> GetSummariesParamContentList(Guid parameter)
        {
            return ExecuteRead<GetSummariesParamContentListQuery, List<SummaryParamContentDTO>, Guid>(parameter);
        }
    }

}
