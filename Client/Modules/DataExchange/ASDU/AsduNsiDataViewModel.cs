using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ASDU;
using GazRouter.DTO.ASDU;
using Microsoft.Practices.Prism.Commands;

namespace DataExchange.ASDU
{
    public enum ChangeTypeKind
    {
        Object = 1,
        Attribute = 2,
        Parameter = 3,
        Analytic = 4,
        Period = 5,
        All = 100
    }

    public class ChangeType
    {
        public ChangeTypeKind ChangeTypeKind { get; }
        public string ChangeTypeName { get; private set; }

        public ChangeType(ChangeTypeKind changeTypeKind, String changeTypeName)
        {
            ChangeTypeKind = changeTypeKind;
            ChangeTypeName = changeTypeName;
        }
    }


    public class AsduNsiDataViewModel : LockableViewModel
    {
        public DelegateCommand LoadDataCommand { get; set; }

        private string _packetKey;

        public string PacketKey
        {
            get { return _packetKey; }
            set { SetProperty(ref _packetKey, value); }
        }

        private ChangeType _selectedChangeType;

        public ChangeType SelectedChangeType
        {
            get { return _selectedChangeType; }
            set
            {
                if (SetProperty(ref _selectedChangeType, value))
                {
                    LoadLog();
                }
            }
        }


        private bool _isDetailsViewEnabled;

        public bool IsDetailsViewEnabled
        {
            get { return _isDetailsViewEnabled; }
            set { SetProperty(ref _isDetailsViewEnabled, value); }
        }

        private bool _isDataLoaded;

        public bool IsDataLoaded
        {
            get { return _isDataLoaded; }
            set { SetProperty(ref _isDataLoaded, value); }
        }


        public AsduNsiDataViewModel()
        {
            _selectedChangeType = ChangeTypes.First();
            OnPropertyChanged(() => SelectedChangeType);
            LogEntries = new ObservableCollection<AsduDataChange>();
            OnPropertyChanged(() => LogEntries);
            LoadDataCommand = new DelegateCommand(LoadLog);
        }

        public IList<ChangeType> ChangeTypes { get; } = new List<ChangeType>
        {
            new ChangeType(ChangeTypeKind.Object, "Объекты"),
            new ChangeType(ChangeTypeKind.Attribute, "Атрибуты"),
            new ChangeType(ChangeTypeKind.Parameter, "Параметры"),
            new ChangeType(ChangeTypeKind.Analytic, "Аналитики"),
            new ChangeType(ChangeTypeKind.Period, "Период")
        };


        public ObservableCollection<AsduDataChange> LogEntries { get; private set; }

        public void Clear()
        {
            LogEntries = new ObservableCollection<AsduDataChange>();
            OnPropertyChanged(() => LogEntries);
        }

        public async void LoadLog()
        {
            if (SelectedChangeType == null || PacketKey == null)
            {
                return;
            }

            IsDataLoaded = true;
            Behavior.TryLock();
            try
            {
                
                var sp = new ASDUServiceProxy();
                BusyMessage = "Выполняется загрузка...";
                LogEntries = new ObservableCollection<AsduDataChange>(await sp.GetImportLogAsync(new GetImportLogParam
                {
                    ImportId = PacketKey,
                    ChangeType = (int) SelectedChangeType.ChangeTypeKind
                }));
                OnPropertyChanged(() => LogEntries);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
    }
}