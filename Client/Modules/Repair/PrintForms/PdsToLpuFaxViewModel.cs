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
    public class PdsToLpuFaxViewModel : AddEditViewModelBase<RepairPlanBaseDTO, int>
    {
        protected override string CaptionEntityTypeName
        {
            get
            {
                return "";
            }
        }

        List<AgreedRepairRecordDTO> Agreed = null;

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

        public int FontOOO { get; set; } = 16;
        public int FontFax { get; set; } = 22;
        public int FontMain { get; set; } = 14;

        private int height_print = 1000;
        private int _height = 800;
        public int height
        {
            get { return _height; }
            set
            {
                if (SetProperty(ref _height, value))
                {
                    OnPropertyChanged(() => height);
                }
            }
        }

        private Guid PdsId;
        private CommonEntityDTO _selectedEntity;
        private RepairPlanBaseDTO _repair;
        public PdsToLpuFaxViewModel(RepairPlanBaseDTO repair, CommonEntityDTO selectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> _agreed) : base(null, null)
        {
            if (UserProfile.Current.Site.IsEnterprise)
            {
                PdsName = UserProfile.Current.Site.Name;
                PdsId = UserProfile.Current.Site.Id;
            }

            _selectedEntity = selectedEntity;
            _repair = repair;

            Perfomer = UserProfile.Current.UserName;

            PrintHelper phelper = new PrintHelper(repair, RepairWorkList);
            Subject = phelper.GetSubjectPermit();

            ObjectName = PdsName + "\n" + selectedEntity.DisplayShortPath;

            //Description = repair.Description;

            //Dates = string.Format("c {0}\nпо\n{1}", 
            //    repair.DateStartSched.HasValue ? repair.DateStartSched.Value.ToShortDateString() : , repair.EndDate.ToShortDateString());

            //Gas = repair.BleedAmount.ToString();

            //Power = "Расчетный объем транспорта газа во время проведения работ" + repair.CalculatedTransfer;

            //Section = selectedEntity.DisplayShortPath;
            

            //Comment = repair.DescriptionGtp;


            LoadUsers();

            //if (_agreed != null)
            //{
            //    Agreed = _agreed;
            //    LoadUsers();
            //}
            

            
            ExportPDFCommand = new DelegateCommand<FrameworkElement>(ExportToPDF);
        }

        private async void LoadUsers()
        {
            var users = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)_selectedEntity.EntityType, FromId = PdsId, IsCpdd = false, ToId = _repair.SiteId });

            try
            {
                var cusers = await new UserManagementServiceProxy().GetUsersAsync();
                Perfomer = UserProfile.Current.UserName + "\n" + cusers.First(o => o.Login == UserProfile.Current.Login).Phone;

            }
            catch
            {
                Perfomer = UserProfile.Current.UserName + "\n";
            }

            string LpuHead = "...Фамилия И.О....";
            string LpuCaption = _repair.SiteName;
            string LpuPos = "Начальнику";
            //DepHead = string.Format("Начальнику\n{0}\nФамилия И.О.", repair.SiteName);
            if (users.To.Count > 0)
            {
                try
                {
                    var head = users.To.First(o => o.IsHead);
                    if (head!=null)
                    {
                        LpuPos = head.Position;
                        LpuHead = head.FIO;
                    }
                }
                catch { }
            }

            DepHead = string.Format("{0}\n{1}\n{2}", LpuPos, LpuCaption, LpuHead);

            string _s = "";
            for (int i=0;i<users.From.Count;i++)
            {
                _s += users.From[i].Position + " \t\t\t\t " + users.From[i].FIO + "\n\n";
            }
            Signers = _s;

            string startdate = _repair.DateStartSched.HasValue ? _repair.DateStartSched.Value.ToShortDateString() : "";
            string enddate = _repair.DateEndSched.HasValue ? _repair.DateEndSched.Value.ToShortDateString() : "";

            var user = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)_selectedEntity.EntityType, FromId = _repair.SiteId, IsCpdd = false, ToId = PdsId });

            
            //DepHead = string.Format("Начальнику\n{0}\nФамилия И.О.", repair.SiteName);
            if (user.From.Count > 0)
            {
                try
                {
                    var head = user.From.First(o => o.IsHead);
                    if (head != null)
                    {
                        LpuPos = head.Position;
                        LpuHead = head.FIO;
                    }
                }
                catch { }
            }

            TextBody = string.Format(" Разрешается с {0} по {1} проведение работ {2} на {3}.\n\tРаботы проводить в строгом соответствии с \"Инструкцией по организации и безопасному проведению огневых работ на объектах {4}\" и \"Инструкцией по организации безопасного проведения газоопасных работ на объектах {4}\".\n\tОтветсвенным за весь комплекс работ и выполнение мероприятий по технике безопасности назначается {5} {6} {7} (с правом утверждения нарядов - допусков)",
                startdate,
                enddate,
                _repair.Description,
                _repair.EntityName,
                PdsName,
                LpuPos,
                _repair.SiteName,
                LpuHead);

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
           
            Signers = _s;
            MTo = signers; */
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

                PdsName = parent.FirstOrDefault(o => o.Id == SiteList[0].ParentId).Name;
            }

           
        }

        public PdsToLpuFaxViewModel(Action<int> actionBeforeClosing, RepairPlanBaseDTO repair, RepairReportDTO report = null)
            : base(actionBeforeClosing, repair)
        {
            
            ValidateAll();
        }

        public PdsToLpuFaxViewModel(Action<int> actionBeforeClosing)
             : base(actionBeforeClosing)
        {
            
        }

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }

        #region header

        private string _depHead = "";
        public string DepHead
        {
            get { return _depHead; }
            set
            {
                if (SetProperty(ref _depHead, value))
                {
                    OnPropertyChanged(() => DepHead);
                }
            }
        }

        private string _textBody = "";
        public string TextBody
        {
            get { return _textBody; }
            set
            {
                if (SetProperty(ref _textBody, value))
                    OnPropertyChanged(() => TextBody);
            }
        }

        

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

        private string _subject = "О проведении работ";
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
            int defheight = height;
            height = height_print;

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

                            height = defheight;
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