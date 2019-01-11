using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.DataExchange.ExchangeEntity;

namespace GazRouter.DataExchange.ASTRA
{
    public class SelectObjectViewModel : DialogViewModel
    {
        #region constructor
        public SelectObjectViewModel(Action<ExchangeEntityDTO> closeCallback, Dictionary<string, ExchangeEntityDTO> qualifyingDict)
            : base(null)
        {
            QualifyingDict = qualifyingDict;
            CreateObjects();
            SelectedObject = QualifyingObjects[0];
            QualifyingDict.TryGetValue(QualifyingObjects[0].Id, out redefinedEntity);
            OkCommand = new DelegateCommand<ExchangeEntityDTO>((s) => { DialogResult = true; closeCallback(redefinedEntity); });
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
            foreach (KeyValuePair<string, ExchangeEntityDTO> kvp in QualifyingDict)
            {
                ExchangeEntityDTO value = kvp.Value;
                objs.Add(new QualifyingObject { Id = kvp.Key, Path = value.EntityName + " (" + value.EntityPath + ")" });
            }
            return objs;
        }

        private Dictionary<string, ExchangeEntityDTO> QualifyingDict;

        private QualifyingObject _selectedObject;
        public QualifyingObject SelectedObject
        {
            get { return _selectedObject; }
            set { _selectedObject = value; QualifyingDict.TryGetValue(_selectedObject.Id, out redefinedEntity); OnPropertyChanged(() => SelectedObject); }
        }

        private ExchangeEntityDTO redefinedEntity = null;

        #endregion
        #region commands
        public DelegateCommand<ExchangeEntityDTO> OkCommand { get; set; }
        #endregion
    }
}


