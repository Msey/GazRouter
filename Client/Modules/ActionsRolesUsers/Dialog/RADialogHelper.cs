using System;
using System.Collections.Generic;
using GazRouter.ActionsRolesUsers.Dialog.AddRoleDialog;
using GazRouter.ActionsRolesUsers.Dialog.AddRoleToUserDialog;
using GazRouter.ActionsRolesUsers.Dialog.AddUserDialog;
using GazRouter.ActionsRolesUsers.Dialog.RemoveRoleDialog;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
using GazRouter.ActionsRolesUsers.Dialog.AddAgreedUserDialog;
using GazRouter.DTO.Repairs.Agreed;

namespace GazRouter.ActionsRolesUsers.Dialog
{
    public static class RADialogHelper
    {
        public static void AddRole(Action<int> callback)
        {
            var viewModel = new AddEditRoleViewModel(callback);
            var view = new AddRoleDialog.AddRoleDialog {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void RemoveRole(Action<int> callback)
        {
            var viewModel = new RemoveRoleViewModel(callback);
            var view = new RemoveRoleDialog.RemoveRoleDialog {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void AddUser(Action<int> callback)
        {
            var viewModel = new AddEditUserViewModel(callback);
            var view = new AddUserDialog.AddUserDialog {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditUser(Action<int> callback, UserDTO userDto)
        {
            var viewModel = new AddEditUserViewModel(callback, userDto);
            var view = new AddUserDialog.AddUserDialog {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void AddAgreedUser(Action<int> callback)
        {
            var viewModel = new AddEditAgreedUserViewModel(callback);
            var view = new AddAgreedUserDialog.AddAgreedUserDialog { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditAgreedUser(Action<int> callback, AgreedUserDTO userDto)
        {
            var viewModel = new AddEditAgreedUserViewModel(callback, userDto);
            var view = new AddAgreedUserDialog.AddAgreedUserDialog { DataContext = viewModel };
            view.ShowDialog();
        }
        public static void RemoveAgreedUser(Action<int> callback)
        {
            var viewModel = new RemoveRoleViewModel(callback);
            var view = new RemoveRoleDialog.RemoveRoleDialog { DataContext = viewModel };
            view.ShowDialog();
        }
        public static void AddRoleToUser(Action<int> callback, UserDTO SelectedUser, List<RoleDTO> UserRoles)
        {
            var viewModel = new AddRoleToUserViewModel(callback) {SelectedUser = SelectedUser, UserRoles = UserRoles};
            var view = new AddRoleToUserDialog.AddRoleToUserDialog {DataContext = viewModel};
            view.ShowDialog();
        }
    }
}