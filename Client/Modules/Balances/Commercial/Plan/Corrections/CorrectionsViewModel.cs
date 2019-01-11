using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Balances.Commercial.Plan.Dialogs;
using GazRouter.Balances.Commercial.Plan.Irregularity;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.Corrections;
using GazRouter.DTO.Balances.Docs;


namespace GazRouter.Balances.Commercial.Plan.Corrections
{
    public class CorrectionsViewModel : ViewModelBase
    {
        private PlanOwnerItem _item;

        public CorrectionsViewModel(ItemBase item, List<DocDTO> docs)
        {
            Items = new List<CorrectionItem>();

            _item = item as PlanOwnerItem;
            if (IsCorrectionsAllowed)
            {
                foreach (var doc in docs)
                {
                    var corItem = new CorrectionItem
                    {
                        DocId = doc.Id,
                        DocDate = doc.CreateDate,
                        DocName = doc.Description,
                        Value = _item?.CorrectionList?.SingleOrDefault(c => c.DocId == doc.Id)?.Value
                    };
                    corItem.PropertyChanged += (obj, args) => OnCorrected();
                    Items.Add(corItem);
                }
            }

            OnPropertyChanged(() => Items);
        }


        private void OnCorrected()
        {
            _item.PlanCorrected = _item.PlanBase + Items.Sum(i => i.Value);
            _item.CorrectionList =
                Items.Where(i => i.Value.HasValue).Select(i => new CorrectionDTO {DocId = i.DocId, Value = i.Value.Value}).ToList();
        }



        // Разрешен ли ввод корректировки для выбранной строки плана
        public bool IsCorrectionsAllowed => _item != null;

        public List<CorrectionItem> Items { get; set; }
    }


    public class CorrectionItem : PropertyChangedBase
    {
        private double? _value;
        public int DocId { get; set; }

        public DateTime DocDate { get; set; }

        public string DocName { get; set; }

        public double? Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
    }
}