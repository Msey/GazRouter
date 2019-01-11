using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.ObjectModel.Model
{
    public class MenuItemViewModel
    {
        public MenuItemViewModel([NotNull] ICommand command)
        {
          Command = command;
        }

        public string Header { get; set; }
        public ICommand Command { get; private set; }
    }

    public class MenuItemCommand : DelegateCommand
    {
        public MenuItemCommand(Action executeMethod) : base(executeMethod)
        {
        }

        public MenuItemCommand(Action executeMethod, Func<bool> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
        }

        public MenuItemCommand(Action executeMethod, Func<bool> canExecuteMethod, string header) : base(executeMethod, canExecuteMethod)
        {
            Header = header;
        }

        public string Header { get; set; }
      
    }


    public class AddEntityMenuItem : MenuItemViewModel
    {
        public AddEntityMenuItem(ICommand command, EntityType type)
            : base(command)
        {
            Type = type;
            Header = ClientCache.DictionaryRepository.EntityTypes.Single(
                et => et.EntityType == type).ShortName;
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        public EntityType Type { get; private set; }
    }

    public class AddPipelineMenuItem : MenuItemViewModel
    {
        public AddPipelineMenuItem(ICommand command, PipelineType type)
            : base(command)
        {
            Type = type;
            Header = ClientCache.DictionaryRepository.PipelineTypes[type].Name;
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public PipelineType Type { get; private set; }
    }

    public class FilterMenuItemCommand : MenuItemCommand, INotifyPropertyChanged
    {
        public FilterMenuItemCommand(Action<EntityType> executeMethod, EntityType type) : base(() => executeMethod(type))
        {
            Type = type;
        }

		public bool InUse
		{
			get { return _inUse; }
			set
			{
				_inUse = value;
				OnPropertyChanged("InUse");
			}
		}

		private bool _inUse;
        public EntityType Type { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

    }
}