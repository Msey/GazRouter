using System;
using System.Collections.Generic;
using System.Windows;
using GazRouter.Controls.Dialogs.DictionaryPicker;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.Controls.Dialogs
{
    public static class DialogHelper
    {

        public static EntityPickerDialogViewModel ShowEntityPicker(Action closeCallback, List<EntityType> allowedTypes)
        {
            var viewModel = new EntityPickerDialogViewModel(closeCallback, allowedTypes);
            var view = new EntityPickerDialogView { DataContext = viewModel };
            view.ShowDialog();
            return viewModel;
        }

        public static DictionaryPickerDialogViewModel ShowDictionaryPicker(Action closeCallback, IEnumerable<BaseDictionaryDTO> listOfItem)
        {
            var viewModel = new DictionaryPickerDialogViewModel(closeCallback, listOfItem);
            var view = new DictionaryPickerDialogView { DataContext = viewModel };
            view.ShowDialog();
            return viewModel;
        }

        public static TreeEntityPickerViewModel ShowTreeEntityPicker(Action<CommonEntityDTO> closeCallback, List<EntityType> allowedTypes)
        {
            var viewModel = new TreeEntityPickerViewModel(closeCallback, allowedTypes);
            var view = new TreeEntityPickerView { DataContext = viewModel };
            view.ShowDialog();
            return viewModel;
        }

        public static void ShowErrorWindow(ApplicationUnhandledExceptionEventArgs e, Uri serverUri = null)
        {
            new ErrorWindow(e.ExceptionObject, serverUri).ShowDialog();
        }

     

    }
}
