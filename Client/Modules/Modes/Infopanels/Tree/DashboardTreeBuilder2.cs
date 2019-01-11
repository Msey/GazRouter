using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.Dashboards;
using GazRouter.DTO.Dashboards;
using GazRouter.DTO.Dashboards.Folders;
using Microsoft.Practices.ObjectBuilder2;

namespace GazRouter.Modes.Infopanels.Tree
{
    /// <summary>
    /// 
    /// OperationContextExtensions: -> set users
    /// 
    /// ОГРАНИЧЕНИЯ: 
    ///  
    /// работа с деревом  - происходит по глобальным привилегиям
    /// работа с панелями - происходит по привилегиям 
    ///  
    /// = папки: =
    /// 1.   Структура папок у всех пользователей одинаковая 
    /// 2.   Всю структуру папок видит супер администратор
    /// 3.   Пользователь видит созданные им папки, и папки других пользователей, если в них есть расшаренные панели 
    /// 4.   В папке исключаются повторения дашбордов c одинаковыми Id
    /// 5.   Пользователь может добавить папку - в привилегии Edit    
    /// 6.   Пользователь может изменить представленную ему структуру папок - добавить, удалить, переименовывать - в привилегии Edit
    /// 7.   Пользователь не видит папки созданные другими пользователями, если они пустые (дашборды в них отсутствуют)
    /// 8.   Удаление папки - проверка на наличие скрытых объектов - предупреждение. Нельзя удалить папку если в ней есть скрытые объекты, 
    ///      не предоставлены на просмотр конкретному пользователю!
    /// 
    /// = панели: =
    /// 1.   Супер Админ предоставляет доступ на панели другим пользователям, обладает полным функционалом
    ///      может ограничить доступ к любым объектам в независимости от создателя это объекта
    /// 2.   При добавлении пользователем новой панели - она открывается всем ролям которыми обладает создатель - на редактирование
    /// 3.   Дашборд может распологаться в нескольких папках в дереве (в одном экземпляре в каждой папке)
    /// 4.   Пользователь видит только дашборды которые ему предоставлены в правах 1, 2
    /// 5.   Все существующие ИП будут спрятаны до расшаривания ими пользователем.
    ///  
    /// ФУНКЦИИ:
    /// 0. Обновление дерева
    /// 1. Добавление 
    /// 2. Переименование 
    /// 3. Удаление
    /// 4. Копирование 
    /// 5. Порядок сортировки
    /// 6. Перенос
    /// 7. Назначение привилегий
    ///  
    /// 1. Dashboards.Id и  - Reports.Id  - не совпадают
    /// 2. папки для Excel.Id & Dashes.Id - не совпадают
    /// 3. FolderId может совпадать c DashboardsId !!!
    ///      Поэтому заполнение дерева - невозможно заполнить
    ///      как одну коллекцию ItemBase!
    /// 
    /// каждый объект должен содержать права доступа на уровне ItemBase
    /// 
    /// test: 1. роль 1
    ///       2. ролей несколько, 
    ///       3. нет ролей
    ///       4. разные папки excel - dash
    ///       5. 
    ///       6. привилегии отсутствуют _permissions
    ///       7. 
    ///       8. 
    /// 
    /// test value:
    /// 
    /// ==================
    /// Обновление дерева:
    /// ==================
    /// 1. Загрузка, 
    /// 2. Update(Refresh)
    /// 3. При возникновении исключения?
    /// 4. 
    /// 
    /// 
    /// todo: вынести Tree Drag&Drop в Behaviour
    /// todo: t.row_type - не добавлен в приложение!!! v_dashboards_reports
    /// 
    /// </summary>
    public class DashboardTreeBuilder2 : DashboardTreeBuilderBase
    {
#region constructor
        public DashboardTreeBuilder2(PanelPermission isEditPermission, DashboardTreeViewModel tree)
        {
            Permission   = isEditPermission;
            _tree        = tree;
        }
#endregion
#region variables
        public readonly PanelPermission Permission;
        private readonly DashboardTreeViewModel _tree;
#endregion
#region property
        public int[] UserRoles { get; private set; }
        public Guid UserLpu { get; set; }
#endregion
#region methods
        /// <summary> если пользователь имеет роль администратора то фильтрация элементов
        ///           дерева по привилегиям не производится        
        /// </summary>
        /// <returns></returns>
        public override async Task Build()
        {
            var treeData = await new DashboardServiceProxy()
                .GetDashboardDataAsync(new DashboardDataParameterSets()
                {
                    UserId = Permission.UserId, 
                    SiteId = Permission.SiteId,
                    Filter = Permission.Filter
                });
            UserRoles = treeData.UserRoleIds;
            if (treeData.UserRoleIds.Length == 0) return;
            //
            var treeObjects = GetTreeObjectsForSuperAdmin(treeData);
            if (treeObjects == null) return;
            //
            Permission.SetPermissions(treeData.MaxRolesPermissions);
            BuildFolders(treeObjects.FolderItems);  
            FillFolders(treeObjects.FolderItems, treeObjects.DashesDictionaryItems);
            // фильтрация? on server
            Traversal(e => { e.Childs = new ObservableCollection<ItemBase>(e.Childs
                              .OrderBy(item => item.SortOrder)
                              .ThenBy(item => item.Name));
            });
        }
        /// <summary> папки и панели созданные всеми пользователями </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private TreeObjects GetTreeObjectsForSuperAdmin(DashboardDataDTO d)
        {
            var folderItems     = d.FolderUnionDtos.Select(e => new FolderItem(e) { Tree = _tree })
                                                   .ToDictionary(k => k.Id);
            var dashDtoExts     = d.DashDtos.Select(e => new DashboardItem
            {
                ContentType = InfopanelItemType.Dash,
                Dto = e,
                Tree = _tree,
                Permission = DashPermissionType.Edit
            });
            var excelDtoExts    = d.ExcelDtos.Select(e => new ReportItem
            {
                ContentType = InfopanelItemType.Excel,
                Dto = e,
                Tree = _tree,
                Permission = DashPermissionType.Edit
            });
            var panelsUnion     = dashDtoExts.Concat(excelDtoExts.Cast<ItemBase>())
                                             .ToDictionary(k => k.Id);
            var treObjects = new TreeObjects(folderItems, panelsUnion);
            return treObjects;
        }
        private void BuildFolders(Dictionary<int, FolderItem> folderItems)
        {
            RootItem.Childs.Clear();
            var enumerator = folderItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.Dto.ParentId.HasValue)
                {
                    var parentId = (int) enumerator.Current.Value.Dto.ParentId;
                    if (folderItems.ContainsKey(parentId))
                        folderItems[parentId].Childs.Add(enumerator.Current.Value);
                } else {
                    RootItem.Childs.Add(enumerator.Current.Value);
                }
            }
        }
        private void FillFolders(IDictionary<int, FolderItem> folders, Dictionary<int, ItemBase> dashs)
        {
            dashs.ForEach(e => {
                if (e.Value.ParentId.HasValue) folders[e.Value.ParentId.Value].Childs.Add(e.Value);
                else RootItem.Childs.Add(e.Value);
            });
        }
#endregion
    }
    internal class TreeObjects
    {
        public TreeObjects(Dictionary<int, FolderItem> folderItems,
                           Dictionary<int, ItemBase> dashesDictionaryItems)
        {
            FolderItems           = folderItems;
            DashesDictionaryItems = dashesDictionaryItems;
        }
        public Dictionary<int, FolderItem> FolderItems { get; }
        public Dictionary<int, ItemBase> DashesDictionaryItems { get; set; }
    }
    internal class DashFolderComparer : IEqualityComparer<DashboardFoldersDTO>
    {
        public bool Equals(DashboardFoldersDTO x, DashboardFoldersDTO y)
        {
            return x.DashboardId == y.DashboardId;
        }
        public int GetHashCode(DashboardFoldersDTO obj)
        {
            return obj.DashboardId.GetHashCode();
        }
    }
}
#region trash
#region checks
//        private static List<DashboardGrantDTO2> GetPermissions1()
//        {
//            return  new List<DashboardGrantDTO2>
//            {
//                new DashboardGrantDTO2 { ItemId = 1,  RoleId = 450, Permission = 1 },
//                new DashboardGrantDTO2 { ItemId = 1,  RoleId = 450, Permission = 1 },
//                new DashboardGrantDTO2 { ItemId = 1,  RoleId = 450, Permission = 1 },
//                new DashboardGrantDTO2 { ItemId = 1,  RoleId = 450, Permission = 1 },
//                new DashboardGrantDTO2 { ItemId = 1,  RoleId = 450, Permission = 1 },
//            };
//        }
//        private static List<DashboardGrantDTO2> GetPermissions2()
//        {
//            return new List<DashboardGrantDTO2>
//            {
//                new DashboardGrantDTO2 { ItemId = 899,  RoleId = 450,  Permission = 1 },
//                new DashboardGrantDTO2 { ItemId = 1128, RoleId = 450,  Permission = 2 },
//                new DashboardGrantDTO2 { ItemId = 899,  RoleId = 2405, Permission = 0 },
//                new DashboardGrantDTO2 { ItemId = 1128, RoleId = 2405, Permission = 0 },
//                new DashboardGrantDTO2 { ItemId = 1129, RoleId = 2405, Permission = 1 },
//                new DashboardGrantDTO2 { ItemId = 1129, RoleId = 449, Permission = 1 },
//            };
//        }
/// <summary>  </summary>
/// <param name="act"></param>
/// <returns></returns>
//private static string MyTime(Action act)
//{
//    var t = new Stopwatch();
//    t.Start();
//    act.Invoke();
//    t.Stop();
//    var tt = t.Elapsed;
//    return $"{tt.Minutes:00}:{tt.Seconds:00}:{tt.Milliseconds:00}";
//}
//private static bool Check1(IEnumerable<FolderDTO> folderDtosUnion,
//                           IEnumerable<DashboardDTO> allDashDtos,
//                           IEnumerable<DashboardDTO> allExcelDtos)
//{
//    // проверка пересечения по Id у панелей и фолдеров
//    var foldersId = folderDtosUnion.Select(e => e.Id);
//    var dashesId = allDashDtos.Concat(allExcelDtos).Select(e => e.Id);
//    var intersect = foldersId.Intersect(dashesId).ToArray();
//    return intersect.Length > 0;
//    // множество - выбрать все значения из A которых нет в В!
//    //            var except = enumerable.Except(e1).ToArray();
//}
#endregion
#endregion

