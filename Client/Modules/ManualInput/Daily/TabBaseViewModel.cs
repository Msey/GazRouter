using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;


namespace GazRouter.ManualInput.Daily
{
    public class TabBaseViewModel : LockableViewModel
    {
        private bool _isReadOnly;

        public TabBaseViewModel()
        {
            Items = new List<ItemBase>();
            CheckEntityList = new List<Guid>();
        }


        public string Header { get; set; }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }

        
        public List<ItemBase> Items { get; set; }

        
        // Есть ли объекты для ввода для выбранного ЛПУ
        public bool HasItems => Items.Count > 0;

        public virtual void Refresh(DailyData data, int coef = 1)
        {

        }

        public List<Guid> CheckEntityList { get; set; }
    }
    
}