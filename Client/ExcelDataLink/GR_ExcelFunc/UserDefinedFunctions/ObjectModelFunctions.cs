using System;
using DataProviders.ObjectModel;
using ExcelDna.Integration;
using Utils.Extensions;
namespace GR_ExcelFunc
{
    public class ObjectModelFunctions
    {
        [ExcelFunction(Description = "Наименование объекта")]
        public static string GetEntityNameById(string entityId)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var commonEntityDTO = FuncHelper.ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());
                    return commonEntityDTO.Name;
                }
                return "Неверный идентификатор объекта";
            }
            catch(Exception err)
            {
                return err.ToString();
            }
        }

        [ExcelFunction(Description = "Иерархия объекта (короткий путь)")]
        public static string GetShortObjectPathById(string entityId)
        {
            try{
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var entity = FuncHelper.ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());
                    return entity.ShortPath;
                }
                return "Неверный идентификатор объекта";}
            catch (Exception err){
                return err.ToString();}
        }

        [ExcelFunction(Description = "Иерархия объекта (полный путь)")]
        public static string GetFullObjectPathById(string entityId)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var entity = FuncHelper.ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());
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
        public static string GetEntityTypeById(string entityId)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var entity = FuncHelper.ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());
                    return entity.EntityType.ToString();
                }
                return "Неверный идентификатор объекта";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }
    }
}
