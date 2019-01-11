using System;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Common.ViewModel
{
    public class MenuItemCommand : DelegateCommand
    {
        public string Header { get; private set; }

        public string ToolTip { get; set; }

        public MenuItemCommand(Action executeMethod, string header) : base(executeMethod)
        {
            Header = header;
        }

		public MenuItemCommand(Action executeMethod, Func<bool> canExecute, string header)
			: base(executeMethod, canExecute)
		{
			Header = header; 
		}
    }
}