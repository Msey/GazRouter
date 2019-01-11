using System;
using System.Linq;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel.Measurings;
using JetBrains.Annotations;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.Flobus.VM.Model
{
    // Крановый узел

    public class Valve : PipelineElementOmBase<ValveDTO>, IComparable, IValve
    {
        public Valve(ValveDTO valveDto, [NotNull] Pipeline pipe) : base(valveDto, pipe)
        {
            ValveMeasuring = new ValveMeasuring(Dto);
            ValveMeasuring.NeedRefresh += delegate { OnPropertyChanged(nameof(Tooltip)); };
            PipelineType = pipe.Dto.Type;
        }

        public PipelineType PipelineType { get; set; }
        public string PurposeName
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.ValvePurposes.Single(
                        vpt => vpt.ValvePurpose == Dto.ValvePurposeId).Name;
            }
        }

        public ValveMeasuring ValveMeasuring { get; }

        public override double Km => Dto.Kilometer;
        public double DisplayKm => PipelineType == PipelineType.Main || PipelineType == PipelineType.Distribution ? Math.Round(Km,0) : Math.Round(Km,2);

        public string Tooltip
            =>
                $"{Name}\r\n{PurposeName}\r\nКм. установки: {DisplayKm:0.###}\r\n\r\n{ValveMeasuring?.State.DisplayValue}\r\n{ValveMeasuring?.StateChangingTimestamp.DisplayValue}"
            ;

        public int CompareTo(object obj)
        {
            var other = (Valve) obj;
            return Km.CompareTo(other.Km);
        }
    }
}