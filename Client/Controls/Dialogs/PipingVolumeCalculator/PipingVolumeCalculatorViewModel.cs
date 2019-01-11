using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.Diameters;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Dialogs.PipingVolumeCalculator
{
    public class PipingVolumeCalculatorViewModel : DialogViewModel
    {
        public PipingVolumeCalculatorViewModel(IEnumerable<PipeSection> piping, Action<List<PipeSection>> callback)
            : base(null)
        {
            if (piping == null) piping = new List<PipeSection>();

            PipingList = new List<PipeSectionViewModel>();
            foreach (var DiameterInfo in ClientCache.DictionaryRepository.Diameters)
            {
                List< PipeSectionExtDiameterViewModel> ExtDiameters = ClientCache.DictionaryRepository.ExternalDiameters.Where(ed => ed.InternalDiameterId == DiameterInfo.Id).Select(ed => new PipeSectionExtDiameterViewModel(ed)).ToList();
                if (ExtDiameters.Count > 0)
                {
                    PipeSection Piping = piping.Where(p => p.Diameter == DiameterInfo.DiameterReal).FirstOrDefault();
                    PipeSectionViewModel PSVM = new PipeSectionViewModel(new PipeSection
                    {
                        Diameter = DiameterInfo.DiameterReal,
                        Length = Piping == null ? 0 : Piping.Length,
                    },
                    DiameterInfo.Name, ExtDiameters);
                    if (Piping != null)
                        PSVM.SelectedExternalDiameterID = PSVM.ExternalDiameters.Where(ed => ed.ExternalDiameter == Piping.ExternalDiameter &&
                                                                                           ed.WallThickness == Piping.WallThickness).Select(ed=>ed.ID).FirstOrDefault();
                    PipingList.Add(PSVM);
                }
            }

            PipingList.ForEach(p => p.PropertyChanged += OnLengthChanged);

            AcceptCommand = new DelegateCommand(() =>
            {
                if (callback != null)
                    callback(PipingList.Where(p => p.Length > 0).Select(p => p.Model).ToList());
                DialogResult = true;
            });

        }


        public List<PipeSectionViewModel> PipingList { get; set; }

        public DelegateCommand AcceptCommand { get; set; }

        public double TotalVolume
        {
            get { return PipingList.Sum(p => p.Volume); }
        }


        private void OnLengthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Length")
                OnPropertyChanged(() => TotalVolume);
        }

    }

    public class PipeSectionViewModel : PropertyChangedBase
    {
        public PipeSection Model { get; private set; }

        public string Name { get; private set; }

        public List<PipeSectionExtDiameterViewModel> ExternalDiameters { get; private set; }

        private int _SelectedExternalDiameterID;
        public int SelectedExternalDiameterID
        {
            get
            {
                return _SelectedExternalDiameterID;
            }
            set
            {
                if (SetProperty(ref _SelectedExternalDiameterID, value))
                {
                    var SelectedExternalDiameter = ExternalDiameters.FirstOrDefault(ed => ed.ID == value);
                    if (SelectedExternalDiameter == null)
                    {
                        Model.ExternalDiameter = 0;
                        Model.WallThickness = 0;
                        SelectedExternalDiameterText = "Значение не выбрано";
                    }
                    else
                    {
                        SelectedExternalDiameterText = SelectedExternalDiameter.Text;
                        Model.ExternalDiameter = SelectedExternalDiameter.ExternalDiameter;
                        Model.WallThickness = SelectedExternalDiameter.WallThickness;
                    }
                    OnPropertyChanged(() => Volume);
                    OnPropertyChanged(() => SelectedExternalDiameterText);
                }
            }
        }

        public string SelectedExternalDiameterText { get; set; }
        public double Length
        {
            get { return Model.Length; }
            set
            {
                Model.Length = value;

                OnPropertyChanged(() => Length);
                OnPropertyChanged(() => Volume);
            }
        }

        public double Volume
        {
            get { return Model.Volume; }
        }

        public PipeSectionViewModel(PipeSection pipe, string name, List<PipeSectionExtDiameterViewModel> ExternalDiameters)
        {
            Model = pipe;
            Name = name;
            this.ExternalDiameters = ExternalDiameters;
            if (ExternalDiameters.Count > 0)
                SelectedExternalDiameterID = ExternalDiameters[0].ID;
        }
    }

    public class PipeSectionExtDiameterViewModel : PropertyChangedBase
    {
        private ExternalDiameterDTO _Model;

        public int ID {get { return _Model.Id; }}

        public double ExternalDiameter { get { return _Model.ExternalDiameter; } }

        public double WallThickness { get { return _Model.WallThickness; } }

        public string Text { get { return $"{ExternalDiameter}мм / {WallThickness}мм"; } }

        public PipeSectionExtDiameterViewModel(ExternalDiameterDTO Model)
        {
            _Model = Model;
        }

        //public override bool Equals(object obj)
        //{
        //    PipeSectionExtDiameterViewModel Obj = obj as PipeSectionExtDiameterViewModel;
        //    if (Obj == null)
        //        return false;
        //    else
        //    {
        //        //return _Model.Id == Obj._Model.Id;
        //        return _Model.Equals(Obj._Model);
        //    }
        //}

        //public override int GetHashCode()
        //{
        //    //return _Model.Id.GetHashCode();
        //    return _Model.GetHashCode();
        //}
    }
}
