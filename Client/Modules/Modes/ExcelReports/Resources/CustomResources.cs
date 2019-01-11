using System.ComponentModel;

namespace GazRouter.Modes.ExcelReports.Resources
{
    public class CustomResources : INotifyPropertyChanged
    {
        private static ExcelReportCustomResouces _strings = new ExcelReportCustomResouces();
        public ExcelReportCustomResouces LocalizedStrings
        {
            get { return _strings; }
            set { OnPropertyChanged("LocalizedStrings");}
        }
     
       #region INotifyPropertyChanged Members
    
       public event PropertyChangedEventHandler PropertyChanged;
    
       private void OnPropertyChanged(string propertyName)
       {
           if (PropertyChanged != null)
           {
               PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
           }
       }
       #endregion
   }
}
