using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Authorization.Role;
namespace GazRouter.Common
{
    /// <summary> заполнение свойств </summary>
    public class Register
    {
#region constructor
        public static Register Instance { get; } = new Register();
        public Register()
        {
            PermissionsMenu = new List<PermissionDTO>();            
        }
#endregion
#region variables
        private static int _parentId;
        public const string Root = "_root";
        public static int RootId => GetId(Root);
        public List<PermissionDTO> PermissionsMenu { get; }
#endregion


        public void AddModule(string name)
        {
            var itemId = GetId(name, Root);
            PermissionsMenu.Add(new PermissionDTO
            {
                ItemId = itemId, //++_linkId,
                ParentId = RootId,
                Name = name
            });
            _parentId = itemId;
        }
        /// <summary> добавление функции авторизации </summary>
        /// <param name="name"></param>
        public PermissionDTO AddLink(string name)
        {
            var itemId = GetId(name, PermissionsMenu.Single(e => e.ItemId == _parentId).Name);
            var permission = new PermissionDTO
            {
                ItemId = itemId, //++_linkId,
                ParentId = _parentId,
                Name = name
            };
            PermissionsMenu.Add(permission);
            return permission;
        }
        public static int GetId(string name, string parentName = "")
        {
            return Math.Abs((parentName + name).GetHashCode());
        }
    }
}
#region trash
//        private static int _linkId;

// _typeViewModelToLink.Add(type, 
// PermissionsMenu[PermissionsMenu.Count - 1].ItemId);
#endregion