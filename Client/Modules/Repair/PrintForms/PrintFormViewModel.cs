using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using GazRouter.Common.ViewModel;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;

namespace GazRouter.Repair.PrintForms
{
    public class PrintFormViewModel : DialogViewModel<Action>
    {
        private readonly IFaxDocFormatter _DocFormatter;
        private RadDocument _RDocument;

        private string _resultText;
        public string resultText
        {
            get
            {
                return _resultText;
            }
            set {
                SetProperty(ref _resultText, value);
            }
        }

        private readonly DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand => _SaveCommand;

        public PrintFormViewModel(IFaxDocFormatter DocFormatter) : base(null)
        {
            _DocFormatter = DocFormatter;
            _SaveCommand = new DelegateCommand(SaveDocument, () => _RDocument != null);

            CreateDocument();
        }

        private async void CreateDocument()
        {
            Lock("Формирование печатного документа");
            try
            {
                _RDocument = await _DocFormatter.CreatePrintDocument();
                resultText = "Печатный документ успешно сформирован, нажмите кнопку \"Сохранить\".";
            }
            catch (Exception Ex)
            {
                resultText = "Произошла ошибка в ходе формирования печатного документа: "+Ex.Message;
                if(IsDebug)
                    throw (Ex);
            }
            finally
            {
                Unlock();
                _SaveCommand.RaiseCanExecuteChanged();
            }

        }

        private void SaveDocument()
        {
            SaveFileDialog SFD = new SaveFileDialog();
            //SFD.DefaultFileName = "Exported Data";
            SFD.Filter = "Документы Word (*.docx)|*.docx";
            SFD.DefaultExt = "docx";
            if (SFD.ShowDialog() == true)
            {
                using (Stream FileStream = SFD.OpenFile())
                {
                    DocxFormatProvider DFormatProvider = new DocxFormatProvider();
                    DFormatProvider.Export(_RDocument, FileStream);
                }
                DialogResult = true;
            }
        }

        protected override void InvokeCallback(Action closeCallback)
        {
            if (closeCallback != null)
                closeCallback();
        }
    }
}
