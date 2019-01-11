using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Dictionaries.RepairTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.SeriesData.PropertyValues;
using Telerik.Windows.Diagrams.Core;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.Application;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.Repairs.Agreed;
using System.Windows.Controls;
using System.Windows;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model;
using System.Windows.Browser;
using System.Windows.Media.Imaging;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;

namespace GazRouter.Repair.PrintForms
{
    public class PdsToCpddFaxViewModel : AddEditViewModelBase<RepairPlanBaseDTO, int>
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

        CommonEntityDTO _selectedEntity = null;

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

        

        public PdsToCpddFaxViewModel(RepairPlanBaseDTO repair, CommonEntityDTO selectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> _agreed) : base(null, null)
        {
            if (UserProfile.Current.Site.IsEnterprise)
            {
                PdsName = UserProfile.Current.Site.Name;
            }
            GetDiameter(repair, RepairWorkList);

            PrintHelper phelper = new PrintHelper(repair, RepairWorkList);
            Subject = phelper.GetSubjectRequest();

            //Perfomer = UserProfile.Current.UserName;

            _selectedEntity = selectedEntity;
            ObjectName = PdsName + "\n" + selectedEntity.DisplayShortPath;

            Description = repair.Description;

            //Dates = string.Format("c {0}\nпо\n{1}", repair.StartDate.ToShortDateString(), repair.EndDate.ToShortDateString());
            Dates = string.Format("c {0}\nпо\n{1}", 
                repair.WFWState.IsProlongation ? repair.DateEndSched.Value.ToShortDateString() : repair.DateStartSched.Value.ToShortDateString(),
                repair.WFWState.IsProlongation ?  "..." : repair.DateEndSched.Value.ToShortDateString());

            Gas = repair.BleedAmount.ToString();

            Power = "Факт: ---\nТВПС: " + repair.CalculatedTransfer;

            Section = phelper.TurnOffSegments();
            

            Comment = repair.DescriptionGtp;

            

            foreach (RepairWork work in RepairWorkList)
            {
                //work.Dto.
            }


            if (_agreed != null)
            {
                Agreed = _agreed;
                LoadUsers();
            }


            FaxDate = "от  " + DateTime.Today.ToLongDateString();


            ExportPDFCommand = new DelegateCommand<FrameworkElement>(ExportToPDF);
        }

        private async void LoadUsers()
        {
            var users = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)_selectedEntity.EntityType, ToId = UserProfile.Current.Site.Id, IsCpdd = true });

            try
            {
                var cusers = await new UserManagementServiceProxy().GetUsersAsync();
                Perfomer = UserProfile.Current.UserName + "\n" + cusers.First(o => o.Login == UserProfile.Current.Login).Phone;

            }
            catch
            {
                Perfomer = UserProfile.Current.UserName + "\n";
            }

            if (users.To.Count > 0)
            {
                Dep1 = users.To[0].Position;
                Dep1Name = users.To[0].FIO;
                Dep1Fax = users.To[0].Fax;

                if (users.To.Count > 1)
                {
                    for (int i = 1; i < users.To.Count; i++)
                    {
                        Dep2 += users.To[i].Position + "\n";
                        Dep2Name += users.To[i].FIO + "\n";
                        Dep2Fax += users.To[i].Fax + "\n";
                        Dep2Sign += users.To[i].Position + " ____________ " + users.To[i].FIO + "\n\n";
                    }
                }
            }

            

            string _s = "";
            for (int i = 0; i < users.From.Count; i++)
            {
                _s += users.From[i].Position + " _____________________ " + users.From[i].FIO + "\n\n";
            }


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
            */
            Signers = _s;
            //MTo = signers;
        }

        private string remont_name = "ремонтируется";
        public async void GetDiameter(RepairPlanBaseDTO _repair, RepairWorkList _works)
        {
            string result = " - ";

            if (_repair.EntityType == DTO.Dictionaries.EntityTypes.EntityType.Pipeline)
            {
                result = "";
                try
                {

                    var segments = await new ObjectModelServiceProxy().GetDiameterSegmentListAsync(_repair.EntityId);
                    if (segments.Count == 1)
                    {
                        result = segments[0].DiameterReal + " мм";
                    }
                    foreach (var work in _works)
                    {
                        if (work.IsSelected && work.Dto.Name.ToLower() == remont_name)
                        {
                            foreach (var s in segments)
                            {
                                if (s.KilometerOfStartPoint <= work.KilometerStart && s.KilometerOfEndPoint >= work.KilometerEnd)
                                {
                                    result = s.DiameterReal + " мм";
                                    break;
                                }
                            }
                        }
                    }

                    if (result == "" && segments.Count > 0)
                    {
                        result = segments[0].DiameterReal + " мм";
                    }
                }
                catch { }
            }

            Diametr = result;
            //return result;
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
        #region To

        private string _dep1 = "";
        public string Dep1
        {
            get { return _dep1; }
            set
            {
                if (SetProperty(ref _dep1, value))
                    OnPropertyChanged(() => Dep1);
            }
        }

        private string _dep2 = "";
        public string Dep2
        {
            get { return _dep2; }
            set
            {
                if (SetProperty(ref _dep2, value))
                    OnPropertyChanged(() => Dep2);
            }
        }

        private string _dep2Sign = "";
        public string Dep2Sign
        {
            get { return _dep2Sign; }
            set
            {
                if (SetProperty(ref _dep2Sign, value))
                    OnPropertyChanged(() => Dep2Sign);
            }
        }
        

        private string _dep3 = "Департамент 338";
        public string Dep3
        {
            get { return _dep3; }
            set
            {
                if (SetProperty(ref _dep3, value))
                    OnPropertyChanged(() => Dep3);
            }
        }

        private string _dep1name = "";
        public string Dep1Name
        {
            get { return _dep1name; }
            set
            {
                if (SetProperty(ref _dep1name, value))
                    OnPropertyChanged(() => Dep1Name);
            }
        }

        private string _dep2name = "";
        public string Dep2Name
        {
            get { return _dep2name; }
            set
            {
                if (SetProperty(ref _dep2name, value))
                {
                    OnPropertyChanged(() => Dep2Name);
                    string[] ss = _dep2name.Trim().Split(' ');
                    string res = "";
                    if (ss.Length > 1 && ss[0].Length > 1)
                    {
                        res = ss[0].Substring(0, ss[0].Length - 1) + " ";
                        for (int i = 1; i < ss.Length; i++)
                            res += ss[i];

                        Sign1 = res;
                    }
                }
            }
        }

        private string _dep3name = "";
        public string Dep3Name
        {
            get { return _dep3name; }
            set
            {
                if (SetProperty(ref _dep3name, value))
                {
                    OnPropertyChanged(() => Dep3Name);
                    string[] ss = _dep3name.Trim().Split(' ');
                    string res = "";
                    if (ss.Length > 1 && ss[0].Length > 1)
                    {
                        res = ss[0].Substring(0, ss[0].Length - 1) + " ";
                        for (int i = 1; i < ss.Length; i++)
                            res += ss[i];

                        Sign2 = res;
                    }
                }
            }
        }

        private string _dep1fax = "";
        public string Dep1Fax
        {
            get { return _dep1fax; }
            set
            {
                if (SetProperty(ref _dep1fax, value))
                    OnPropertyChanged(() => Dep1Fax);
            }
        }

        private string _dep2fax = "";
        public string Dep2Fax
        {
            get { return _dep2fax; }
            set
            {
                if (SetProperty(ref _dep2fax, value))
                    OnPropertyChanged(() => Dep2Fax);
            }
        }

        private string _dep3fax = "";
        public string Dep3Fax
        {
            get { return _dep3fax; }
            set
            {
                if (SetProperty(ref _dep3fax, value))
                    OnPropertyChanged(() => Dep3Fax);
            }
        }

        #endregion

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
                PdsName = parent.FirstOrDefault(o => o.Id == SiteList[0].ParentId).Name;
            }
        }

        public PdsToCpddFaxViewModel(Action<int> actionBeforeClosing, RepairPlanBaseDTO repair, RepairReportDTO report = null)
            : base(actionBeforeClosing, repair)
        {
            
            ValidateAll();
        }

        public PdsToCpddFaxViewModel(Action<int> actionBeforeClosing)
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

        private string _power = "";
        public string Power
        {
            get { return _power; }
            set
            {
                if (SetProperty(ref _power, value))
                    OnPropertyChanged(() => Power);
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