using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.DataExchange.ASUTP
{
    public class SelectObjectOrParameterViewModel : DialogViewModel
    {
        #region constructor
        public SelectObjectOrParameterViewModel(Action<AsutpViewModel.SpecifiedEntityOrProperty> closeCallback, List<AsutpViewModel.SpecifiedEntityOrProperty> list)
            : base(null)
        {
            List = list;
            CreateObjects();
            SelectedObject = QualifyingObjects[0];
            OkCommand = new DelegateCommand<AsutpViewModel.SpecifiedEntityOrProperty>((s) => { DialogResult = true; closeCallback(redefinedEntityOrProperty); });
        }
        #endregion
        #region variables

        public class QualifyingObject : PropertyChangedBase

        {
            private string _id;
            private string _path;
            public string Id
            {
                get { return _id; }
                set
                {
                    if (value != _id)
                    {
                        _id = value;
                        OnPropertyChanged(() => Id);
                    }
                }
            }
            public string Path
            {
                get { return _path; }
                set
                {
                    if (value != _path)
                    {
                        _path = value;
                        OnPropertyChanged(() => Path);
                    }
                }
            }
        }

        private ObservableCollection<QualifyingObject> _qualifyingobjects;

        public ObservableCollection<QualifyingObject> QualifyingObjects
        {
            get
            {
                if (_qualifyingobjects == null)
                {
                    _qualifyingobjects = CreateObjects();
                }

                return _qualifyingobjects;
            }
            set { }
        }

        private ObservableCollection<QualifyingObject> CreateObjects()
        {
            ObservableCollection<QualifyingObject> objs = new ObservableCollection<QualifyingObject>();
            foreach (AsutpViewModel.SpecifiedEntityOrProperty eop in List)
                objs.Add(new QualifyingObject { Id = eop.extid, Path = eop.path });
            return objs;
        }

        private List<AsutpViewModel.SpecifiedEntityOrProperty> List;

        private QualifyingObject _selectedObject;
        public QualifyingObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                foreach (AsutpViewModel.SpecifiedEntityOrProperty eop in List)
                { if (eop.path == _selectedObject.Path) { redefinedEntityOrProperty = eop; OnPropertyChanged(() => SelectedObject); break; } }
            }
        }

        private AsutpViewModel.SpecifiedEntityOrProperty redefinedEntityOrProperty;

        #endregion
        #region commands
        public DelegateCommand<AsutpViewModel.SpecifiedEntityOrProperty> OkCommand { get; set; }
        #endregion
    }
}


