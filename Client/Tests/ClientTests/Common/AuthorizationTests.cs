using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.DTO.Authorization.Role;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ClientTests.Common
{
    [TestClass]
    [Tag("Common.Authorization")]
    public class AuthorizationTests
    {
        [TestInitialize]
        public void TestInitialize(){}
        [TestMethod]
        public void UnionTest()
        {
            // набор ролей пользователя
            var roles = new List<RoleDTO>
            {
                new RoleDTO {Id = 0, Name = "name1", Description = "d1"},
                new RoleDTO {Id = 1, Name = "name2", Description = "d2"},
//                new RoleDTO {Id = 2, Name = "name3", Description = "d3"}
            };
            var roles2 = new List<RoleDTO>
            {
                new RoleDTO {Id = 0, Name = "name1", Description = "d1"},
            };
            var roles3 = new List<RoleDTO>();
            var data = new List<PermissionDTO>
            {
                new PermissionDTO {ItemId = 1, ParentId = 0, RoleId = 0, Name = "меню 1", Permission = 0 },
                new PermissionDTO {ItemId = 1, ParentId = 0, RoleId = 1, Name = "меню 1", Permission = 1 },
                new PermissionDTO {ItemId = 1, ParentId = 0, RoleId = 2, Name = "меню 1", Permission = 2 },
                //
                new PermissionDTO {ItemId = 2, ParentId = 0, RoleId = 0, Name = "меню 2", Permission = 0 },
                new PermissionDTO {ItemId = 2, ParentId = 0, RoleId = 1, Name = "меню 2", Permission = 1 },
                new PermissionDTO {ItemId = 2, ParentId = 0, RoleId = 2, Name = "меню 2", Permission = 2 },
                //
                new PermissionDTO {ItemId = 3, ParentId = 0, RoleId = 0, Name = "меню 3", Permission = 10 },
                new PermissionDTO {ItemId = 3, ParentId = 0, RoleId = 1, Name = "меню 3", Permission = 1 },
                new PermissionDTO {ItemId = 3, ParentId = 0, RoleId = 2, Name = "меню 3", Permission = 2 },
                //
                new PermissionDTO {ItemId = 4, ParentId = 0, RoleId = 0, Name = "меню 4", Permission = 0 },
                new PermissionDTO {ItemId = 4, ParentId = 0, RoleId = 1, Name = "меню 4", Permission = 1 },
                new PermissionDTO {ItemId = 4, ParentId = 0, RoleId = 2, Name = "меню 4", Permission = 2 },
            };
            // выборка нескольких 

            var select2 = roles2.Join(data,
                                      role => role.Id, d => d.RoleId,
                                     (dto, permissionDTO) => permissionDTO).ToList();
            var select3 = (from itemRole in roles3
                           join itemData in data
                           on itemRole.Id equals itemData.RoleId
                           select itemData).ToList();
            // группировка по Id
            var select1 = roles.Join(data, 
                                     role => role.Id, perm => perm.RoleId,
                                     (dto, perm) => perm);
            var group = select1.GroupBy(e => e.ItemId)
                               .ToDictionary(k => k.Key, 
                                             v => v.Max(f => f.Permission));
            var a = group[3];
            // 
            var t = data.GroupBy(e => e.RoleId).ToDictionary(k=>k.Key, v=>v.ToList());


           var count =  t[0].Count;
           var oc = new ObservableCollection<string>();
            

        }
    }
}
#region trash




//                               .Select(e => new {a = e.Key, b = e.Max(f=>f.Permission)});
//            var union = group.Select(e => new {e.Key, Max = e.Value.Max(d=>d.Permission)});
#endregion
