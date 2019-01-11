using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Flobus.Misc
{
    public static class RadContextMenuHelper
    {
         public static void AddSeparator(this RadContextMenu menu)
         {
             menu.Items.Add(new RadMenuItem{IsSeparator = true});
         }

         public static void AddCommand(this RadContextMenu menu, string header, DelegateCommand commamd)
        {
            menu.Items.Add(
                new RadMenuItem
                {
                    Header = header,
                    Command = commamd,
                });
        }

         public static void AddCommand(this RadContextMenu menu, string header, RoutedUICommand commamd, Point mousePosition)
         {
             menu.Items.Add(
                 new RadMenuItem
                 {
                     Header = header,
                     Command = commamd,
                     CommandParameter = mousePosition
                 });
         }

         public static void AddCommand(this RadContextMenu menu, string header, DelegateCommand<Point?> commamd, Point mousePosition)
         {
             menu.Items.Add(
                 new RadMenuItem
                 {
                     Header = header,
                     Command = commamd,
                     CommandParameter = mousePosition
                 });
             
             
         }


        public static void AddCommand(this RadContextMenu menu, string header, DelegateCommand<MouseButtonEventArgs> commamd, MouseButtonEventArgs mousePosition)
        {
            menu.Items.Add(
                new RadMenuItem
                {
                    Header = header,
                    Command = commamd,
                    CommandParameter = mousePosition
                });
        }

    }
}