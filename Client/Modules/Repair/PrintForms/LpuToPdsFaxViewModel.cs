using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairReport;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Fixed.Model;

namespace GazRouter.Repair.PrintForms
{
    public class LpuToPdsFaxViewModel : AddEditViewModelBase<RepairPlanBaseDTO, int>
    {
        protected override string CaptionEntityTypeName
        {
            get
            {
                return "";
            }
        }

        List<AgreedRepairRecordDTO> Agreed = null;

        public int FontOOO { get; set; } = 16;
        public int FontFax { get; set; } = 22;
        public int FontMain { get; set; } = 14;

        private string _perfomer = "";
        public string Perfomer
        {
            get { return _perfomer; }
            set
            {
                if (SetProperty(ref _perfomer, value))
                    OnPropertyChanged(() => Perfomer);
            }

        }

        Guid PdsId;
        CommonEntityDTO _selectedEntity;
        public LpuToPdsFaxViewModel(RepairPlanBaseDTO repair, CommonEntityDTO selectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> _agreed) : base(null, null)
        {

            if (UserProfile.Current.Site.IsEnterprise)
            {
                PdsName = UserProfile.Current.Site.Name;
            }
            else
            {
                LpuName = UserProfile.Current.Site.Name;
                GetEnterprise();
            }

            PrintHelper phelper = new PrintHelper(repair, RepairWorkList);
            Subject = phelper.GetSubjectRequest();
            Segment = phelper.TurnOffSegments();            

            _selectedEntity = selectedEntity;
            ObjectName = PdsName + "\n" + selectedEntity.DisplayShortPath;

            Description = repair.Description;

            Potrebitel = repair.ConsumersState;

            //Dates = string.Format("c {0}\nпо\n{1}", repair.StartDate.ToShortDateString(), repair.EndDate.ToShortDateString());

            if (repair.WFWState.IsProlongation)
            {
                Dates = string.Format("c {0} {1}:{2}\nпоДАТА/ВРЕМЯ\n\nпродолжительность: ... ч.\n", repair.DateEndSched.Value.ToShortDateString(), repair.DateEndSched.Value.Hour, repair.DateEndSched.Value.Minute);
            }
            else
            {
                Dates = string.Format("c {0} {1}:{2}\nпо\n{3} {4}:{5}\nпродолжительность: {6} ч.\n", repair.DateStartSched.Value.ToShortDateString(), repair.DateStartSched.Value.Hour, repair.DateStartSched.Value.Minute, repair.DateEndSched.Value.ToShortDateString(), repair.DateEndSched.Value.Hour, repair.DateEndSched.Value.Minute, ((TimeSpan)(repair.DateEndSched - repair.DateStartSched)).TotalHours);
            }


            

            Gas = repair.BleedAmount.ToString();

            //Power = "Расчетный объем транспорта газа во время проведения работ" + repair.CalculatedTransfer;

            Section = selectedEntity.DisplayShortPath;


            Comment = repair.DescriptionGtp;

            Plan = repair.GazpromPlanID;

            if (_agreed != null)
            {
                Agreed = _agreed;
                
            }

            LoadUsers();

            FaxDate = "№ _________ от  " + DateTime.Today.ToLongDateString();
            ExportPDFCommand = new DelegateCommand<FrameworkElement>(ExportToPDF);
        }

        private string _faxDate = "";
        public string FaxDate
        {
            get { return _faxDate; }
            set
            {
                if (SetProperty(ref _faxDate, value))
                    OnPropertyChanged(() => FaxDate);
            }
        }

        private async void LoadUsers()
        {
            
            var users = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)_selectedEntity.EntityType, FromId = UserProfile.Current.Site.Id, IsCpdd = false, ToId = PdsId });

            try
            {
                var cusers = await new UserManagementServiceProxy().GetUsersAsync();
                Perfomer = UserProfile.Current.UserName + "\n" + cusers.First(o => o.Login == UserProfile.Current.Login).Phone;

            }
            catch
            {
                Perfomer = UserProfile.Current.UserName + "\n";
            }

            string __to = "";
            for (int i = 0; i < users.To.Count; i++)
            {
                __to += users.To[i].Position + " \t\t" + users.To[i].FIO + "\n";
            }
            To = __to;

            string _s = "";
            for (int i = 0; i < users.From.Count; i++)
            {
                string signspacer = " _____________________ ";
                try
                {
                    var a = Agreed.First(o => o.AgreedUserName == users.From[i].FIO || o.RealAgreedUserName == users.From[i].FIO);
                    if (a.AgreedResult.HasValue && a.AgreedResult.Value)
                        signspacer = "    Согласованно  ";
                }
                catch { }
                _s += users.From[i].Position + signspacer  + users.From[i].FIO + "\n\n";
               
            }

            
            Signers = _s;
            /*
            var users = await new UserManagementServiceProxy().GetUsersAsync();
            var _AgreementPersons = await new UserManagementServiceProxy().GetAgreedUsersAsync(DateTime.Now);
            //Users = new ObservableCollection<UserDTO>(users);

            List<Persons> signers = new List<Persons>();
            string _s = "";
            foreach (var a in Agreed)
            {
                var auc = _AgreementPersons.FirstOrDefault(au => au.AgreedUserId == a.AgreedUserId);
                if (auc != null)
                {
                    try
                    {
                        var us = users.FirstOrDefault(u => u.Id == auc.UserID);
                        if (us != null && us.UserName != "")
                        {
                            if (us.SiteName != PdsName)
                            {

                            }
                            else
                            {
                                _s += a.AgreedUserPosition + " \t\t\t\t " + a.AgreedUserName + "\n";
                                signers.Add(new Persons() { Position = a.AgreedUserPosition, FIO = a.AgreedUserName });
                            }
                        }
                    }
                    catch { }
                } 
                
            }

            To = _s;
            MTo = signers;
            */
        }

        private async void GetEnterprise()
        {
            var SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    SiteId = UserProfile.Current.Site.Id
                });

            if (SiteList != null && SiteList.Count == 1)
            {
                var parent = await new ObjectModelServiceProxy().GetEnterpriseListAsync();
                try { PdsId = SiteList[0].ParentId.Value; } catch { }
                PdsName = parent.FirstOrDefault(o => o.Id == SiteList[0].ParentId).Name;
            }

            LoadUsers();

           
        }

        public LpuToPdsFaxViewModel(Action<int> actionBeforeClosing, RepairPlanBaseDTO repair, RepairReportDTO report = null)
            : base(actionBeforeClosing, repair)
        {
            
            ValidateAll();
        }

        public LpuToPdsFaxViewModel(Action<int> actionBeforeClosing)
             : base(actionBeforeClosing)
        {
            
        }

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }

        #region header

        private string _pdsName = "";
        public string PdsName
        {
            get { return _pdsName; }
            set
            {
                if (SetProperty(ref _pdsName, value))
                {
                    OnPropertyChanged(() => PdsName);
                }
            }
        }

        private string _signers = "";
        public string Signers
        {
            get { return _signers; }
            set
            {
                if (SetProperty(ref _signers, value))
                    OnPropertyChanged(() => Signers);
            }
        }

        private string _signersTable = "";
        public string signersTable
        {
            get { return _signersTable; }
            set
            {
                if (SetProperty(ref _signersTable, value))
                    OnPropertyChanged(() => signersTable);
            }
        }
        

        private string _to = "";
        public string To
        {
            get { return _to; }
            set
            {
                if (SetProperty(ref _to, value))
                    OnPropertyChanged(() => To);
            }
        }


        private string _subject;
        public string Subject
        {
            get { return _subject; }
            set
            {
                if (SetProperty(ref _subject, value))
                {
                    OnPropertyChanged(() => Subject);
                }
            }
        }
        #endregion
       
        #region Table

        private string _objectName = "";
        public string ObjectName
        {
            get { return _objectName; }
            set
            {
                if (SetProperty(ref _objectName, value))
                    OnPropertyChanged(() => ObjectName);
            }
        }

        private string _diametr = "";
        public string Diametr
        {
            get { return _diametr; }
            set
            {
                if (SetProperty(ref _diametr, value))
                    OnPropertyChanged(() => Diametr);
            }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                    OnPropertyChanged(() => Description);
            }
        }

        private string _dates = "";
        public string Dates
        {
            get { return _dates; }
            set
            {
                if (SetProperty(ref _dates, value))
                    OnPropertyChanged(() => Dates);
            }
        }

        private string _gas = "";
        public string Gas
        {
            get { return _gas; }
            set
            {
                if (SetProperty(ref _gas, value))
                    OnPropertyChanged(() => Gas);
            }
        }

        private string _plan = "";
        public string Plan
        {
            get { return _plan; }
            set
            {
                if (SetProperty(ref _plan, value))
                    OnPropertyChanged(() => Plan);
            }
        }

        private string _potrebitel = "Потребитель";
        public string Potrebitel
        {
            get { return _potrebitel; }
            set
            {
                if (SetProperty(ref _potrebitel, value))
                    OnPropertyChanged(() => Potrebitel);
            }
        }

        private string _segment = "";
        public string Segment
        {
            get { return _segment; }
            set
            {
                if (SetProperty(ref _segment, value))
                    OnPropertyChanged(() => Segment);
            }
        }

        private string _section = "";
        public string Section
        {
            get { return _section; }
            set
            {
                if (SetProperty(ref _section, value))
                    OnPropertyChanged(() => Section);
            }
        }

        #endregion

        #region Footer

        private bool _isPrintHasComment = true;
        public bool IsPrintHasComment
        {
            get { return _isPrintHasComment; }
            set
            {
                if (SetProperty(ref _isPrintHasComment, value))
                {
                    OnPropertyChanged(() => IsPrintHasComment);
                }
            }
        }
        private string _comment = "";
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (SetProperty(ref _comment, value))
                {
                    OnPropertyChanged(() => Comment);
                }
            }
        }

        private string _sign1 = "Виденеев А.Н.";
        public string Sign1
        {
            get { return _sign1; }
            set
            {
                if (SetProperty(ref _sign1, value))
                    OnPropertyChanged(() => Sign1);
            }
        }

        private string _sign2 = "Булычев Н.И.";
        public string Sign2
        {
            get { return _sign2; }
            set
            {
                if (SetProperty(ref _sign2, value))
                    OnPropertyChanged(() => Sign2);
            }
        }


        #endregion
        private string _lpuName = "";
        public string LpuName {
            get { return _lpuName; }
            set {
                if (SetProperty(ref _lpuName, value))
                {
                    OnPropertyChanged(() => LpuName);
                }
            }
        }

        

        private List<Persons> _mTo = new List<Persons>();
        public List<Persons> MTo {
            get { return _mTo; }
            set
            {
                if (SetProperty(ref _mTo, value))
                {
                    OnPropertyChanged(() => MTo);
                }
            }
        }


        private List<Persons> _signature = new List<Persons>();
        public List<Persons> Signature
        {
            get { return _signature; }
            set
            {
                if (SetProperty(ref _signature, value))
                {
                    OnPropertyChanged(() => Signature);
                }
            }
        }

        



        private Microsoft.Practices.Prism.Commands.DelegateCommand _printCommand;
        public Microsoft.Practices.Prism.Commands.DelegateCommand PrintCommand => _printCommand;
       
        private bool _isPrint = false;
        public bool IsPrint
        {
            get { return _isPrint; }
            set
            {
                if (SetProperty(ref _isPrint, value)) OnPropertyChanged(() => IsPrint);
            }
        }

        public DelegateCommand<FrameworkElement> ExportPDFCommand { get; set; }

        public async void ExportToPDF(FrameworkElement content)
        {
            
            var ganttView = (Grid)content;// FindChild(content, x => x is Grid) as Grid;
            var dlg = new SaveFileDialog() { Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1, /*DefaultFileName = Header*/ };
            if (dlg.ShowDialog() == true)
            {
                IsPrint = true;
                if (Comment == null || Comment.Trim() == "") IsPrintHasComment = false;

                var stream = dlg.OpenFile();
                this.Lock("Экспорт в PDF...");
                var bw = new System.ComponentModel.BackgroundWorker();
                bw.DoWork += (s, e) => {
                    System.Threading.Thread.Sleep(1000);
                    ganttView.Dispatcher.BeginInvoke(() => {
                        try
                        {
                            
                            using (stream)
                            {
                                var document = new RadFixedDocument();
                                var pageSize = new Size(ganttView.RenderSize.Width, ganttView.RenderSize.Height);//. 1488.58, 1056);//new Size(796.8, 1123.2)

                                var image = BitmapUtils.CreateWriteableBitmap(ganttView,
                                    new Rect()
                                    { Height = ganttView.RenderSize.Height, Width = ganttView.RenderSize.Width, X = 0, Y = 0 },
                                    new Size(ganttView.RenderSize.Width, ganttView.RenderSize.Height),
                                    ganttView.Background, new Thickness());
                                RadFixedPage page = new RadFixedPage();
                                page.Size = pageSize;
                                //page.Rotation = Telerik.Windows.Documents.Fixed.Model.Data.Rotation.Rotate90;
                                page.Content.AddImage(new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(image, ImageQuality.High));
                                document.Pages.Add(page);
                                new PdfFormatProvider().Export(document, stream);

                                stream.Flush();
                            }
                        }
                        finally
                        {
                            this.Unlock();

                            IsPrint = false;
                            IsPrintHasComment = true;
                        }
                    });
                };
                bw.RunWorkerAsync();
            }
        }

        private static DependencyObject FindChild(DependencyObject dObject, Func<DependencyObject, bool> predicate)
        {
            var fElement = dObject as FrameworkElement;
            if (fElement != null)
            {
                int cCount = VisualTreeHelper.GetChildrenCount(fElement);
                for (int i = 0; i < cCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(fElement, i);
                    if (predicate(child)) return child;
                    var v = FindChild(child, predicate);
                    if (v != null) return v;
                }
            }
            return null;
        }

    }
};