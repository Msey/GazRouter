using System;
using System.Linq;
using System.Reflection;
using DataProviders;
using DataProviders.ObjectModel;
using ExcelDna.Integration;
using Utils.Extensions;

namespace GR_ExcelFunc.UDF
{
    public class ObjectModelSync : BaseSync
    {
        [ExcelFunction(Description = "Наименование объекта")]
        public static string EntityNameById(string entityId)
        {
            try
            {
                //var str = serv.GetSiteList(new GazRouter.DTO.ObjectModel.Sites.GetSiteListParameterSet { SystemId = 1 });
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var entity = ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());
                    return entity.Name;
                }
                return "Неверный идентификатор объекта";
            }
            catch(Exception err)
            {
                return err.ToString();
            }
        }

        [ExcelFunction(Description = "Иерархия объекта (короткий путь)")]
        public static string ShortObjectPathById(string entityId)
        {
            try{
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var entity = ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());
                    return entity.ShortPath;
                }
                return "Неверный идентификатор объекта";}
            catch (Exception err){
                return err.ToString();}
        }

        [ExcelFunction(Description = "Иерархия объекта (полный путь)")]
        public static string FullObjectPathById(string entityId)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var entity =ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert()) ;
                    return entity.Path;
                }
                return "Неверный идентификатор объекта";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }
        [ExcelFunction(Description = "Тип объекта")]
        public static string EntityTypeById(string entityId)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var entity = ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());
                    return entity.EntityType.ToString();
                }
                return "Неверный идентификатор объекта";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }

        [ExcelFunction(Description = "Версия клиентского ПО")]
        public static string ClientVersion()
        {
            return
                Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(false)
                    .OfType<AssemblyFileVersionAttribute>()
                    .Single()
                    .Version;
        }
    }
}
