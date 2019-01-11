using System;
using System.Collections.Generic;
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
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ASDU;
using Microsoft.Practices.Prism.Commands;

namespace DataExchange.ASDU
{
    public class AsduXmlFileViewModel : LockableViewModel
    {
        public DelegateCommand SaveXmlCommand { get; private set; }
        public AsduXmlFileViewModel(LoadedFileWrapper selectedFile)
        {
            SelectedFile = selectedFile;
            SaveXmlCommand = new DelegateCommand(SaveXml);
        }

        private void SaveXml()
        {
            if (_xml != null)
            {
                    var textDialog = new SaveFileDialog
                    {
                        Filter = "Файлы xml | *.xml",
                        DefaultExt = "xml",
                        DefaultFileName = SelectedFile.FileName
                    };
                    var result = textDialog.ShowDialog();
                    if (result == true)
                    {
                        var fileStream = textDialog.OpenFile();
                        var sw = new System.IO.StreamWriter(fileStream);
                        sw.WriteLine(_xml);
                        sw.Flush();
                        sw.Close();
                    }
            }
        }

        private string _xml;

        private IList<string> _fileXml;

        public IList<string> FileXml
        {
            get { return _fileXml; }
            set { SetProperty(ref _fileXml, value); }
        }

        private bool _isXmlLoading;

        public bool IsXmlLoading
        {
            get { return _isXmlLoading; }
            set { SetProperty(ref _isXmlLoading, value); }
        }


        public LoadedFileWrapper SelectedFile { get; private set; }

        public async void ShowXml()
        {
            if (SelectedFile == null)
            {
                return;
            }

            FileXml = null;
            var tmpList = new List<string>();
            IsXmlLoading = true;
            try
            {
                _xml = null;
                var sp = new ASDUServiceProxy();
                _xml = await sp.GetLoadedFileXmlAsync(SelectedFile.LoadedFile);
                using (var reader = new StringReader(_xml))
                {
                    string line;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            tmpList.Add(line);
                        }

                    } while (line != null);
                }

                FileXml = tmpList;

            }
            finally
            {
                IsXmlLoading = false;
            }
        }
    }
}
