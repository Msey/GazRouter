using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Authorization.Role;
namespace GazRouter.Common
{
    /// <summary> А  В  Т  О  Р  И  З  А  Ц  И  Я
    /// 
    /// 1. у пользователя могут быть несколько ролей
    /// 2. пользователю назначается наивысшая привилегия 
    ///    из имеющихся в списке ролей пользователя
    /// 
    /// </summary>
    public class Authorization
    {
#region constructor
        public static Authorization Instance { get; } = new Authorization();
        public Authorization()
        {
            _viewModelToLink = new Dictionary<Type, int>();
        }
        public void Init(List<RoleDTO> userRoles, IEnumerable<PermissionDTO> permissions)
        {
            _userRoles = userRoles;
            _dbUnionPermissionses = 
                GetUnionDbPermissions(userRoles.Select(e => e.Id), permissions);
        }
#endregion
#region variables
        private readonly Dictionary<Type, int> _viewModelToLink;
        private Dictionary<int, PermissionType> _dbUnionPermissionses;
        private List<RoleDTO> _userRoles;
#endregion
#region auth
        public List<RoleDTO> GetUserRoles()
        {
            return _userRoles;
        }
        private static Dictionary<int, PermissionType> 
            GetUnionDbPermissions(IEnumerable<int> userRoles, 
                                  IEnumerable<PermissionDTO> permissions)
        {
            var select = userRoles.Join(permissions,
                                        role => role, 
                                        perm => perm.RoleId,
                                        (dto, perm) => perm);
            //
            return select.GroupBy(e => e.ItemId)
                               .ToDictionary(k => k.Key,
                                             v => (PermissionType)v.Max(f => f.Permission));
        }
        public void AddModule(string name)
        {
            Register.Instance.AddModule(name);
        }
        /// <summary>
        /// 
        /// группа ViewModel объединена в пределах одной ссылки
        /// все классы этой группы будут иметь одну привилегию!
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public PermissionType AddLink(string name, Type[] types)
        {
            var permission = Register.Instance.AddLink(name);
            foreach (var type in types)                            
                _viewModelToLink.Add(type, permission.ItemId);// ! привязка один к одному // if (!_viewModelToLink.ContainsKey(type)) !! todo:
            return IsAuthorized(types.First());
        }
        /// <summary> если запрещающей записи нет в базе, 
        /// то ссылка будет доступна </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PermissionType IsAuthorized(Type type)
        {
            return !_viewModelToLink.ContainsKey(type) ?
                                    PermissionType.Write : 
                                    GetPermission(_viewModelToLink[type]);
        }
        public PermissionType GetPermission(int linkId)
        {
            return _dbUnionPermissionses.ContainsKey(linkId) ? 
                       _dbUnionPermissionses[linkId] : PermissionType.Write;
        }
#endregion
    }
}
#region trash
//private Dictionary<LinkType, string> GetGuidDictionary()
//{
//    return new Dictionary<LinkType, string>
//            {
//                { LinkType.Schema, "" },
//
//
//            };
//}
#endregion
