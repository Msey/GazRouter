using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using GazRouter.DTO.Repairs.Agreed;

namespace GazRouter.Repair.Agreement
{
    public class AgreementListViewRowStyleSelector:StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is AgreedRepairRecordDTO)
            {
                AgreedRepairRecordDTO Record = item as AgreedRepairRecordDTO;
                bool IsAgreeingAllowed = false;
                var Row = container as Telerik.Windows.Controls.GridView.GridViewRow;
                if (Row != null)
                {
                    var VM = Row.GridViewDataControl.DataContext as AgreementListViewModel;
                    if (VM != null)
                        IsAgreeingAllowed = VM.IsAgreeingAllowed(Record);
                }
                if (Record.AgreedResult.HasValue)
                {
                    if (Record.AgreedResult.Value) return IsAgreeingAllowed ? AgreeingAgreedRowStyle : AgreedRowStyle;
                    else
                        return IsAgreeingAllowed ? AgreeingDisagreedRowStyle : DisagreedRowStyle;
                }
                else
                {
                    if (IsAgreeingAllowed)
                    {
                         return AgreeingRowStyle;
                    }
                }
            }
            return null;
        }
        public Style AgreedRowStyle { get; set; }
        public Style DisagreedRowStyle { get; set; }
        public Style AgreeingRowStyle { get; set; }
        public Style AgreeingAgreedRowStyle { get; set; }
        public Style AgreeingDisagreedRowStyle { get; set; }
    }
}
