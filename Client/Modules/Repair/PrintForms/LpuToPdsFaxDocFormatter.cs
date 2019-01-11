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
using GazRouter.DTO.Repairs.PrintForms;
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
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Model.Styles;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;

namespace GazRouter.Repair.PrintForms
{
    public class LpuToPdsFaxDocFormatter : FaxDocFormatterBase
    {
        public LpuToPdsFaxDocFormatter(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
            :base(Repair, SelectedEntity, RepairWorkList, AgreedList)
        {
        }
        public LpuToPdsFaxDocFormatter(RadDocument DocumentPattern, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
            :base(DocumentPattern, Repair, SelectedEntity, RepairWorkList, AgreedList)
        {
        }

        protected override async Task<byte[]> LoadRemoteDocPattern()
        {
            return await new RepairsServiceProxy().GetPrintDocumentPatternAsync("LpuToPds");
        }

        private SignersDTO _Signers;
        protected override async Task<Dictionary<string, string>> GetParams(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            PrintHelper phelper = new PrintHelper(Repair, RepairWorkList);

            Guid PdsId=new Guid();
            string PdsName ="";
            string LpuName ="";
            string FaxDate = DateTime.Today.ToLongDateString();
            string targetPos = "";
            string targetName = "";
            string Subject = phelper.GetSubjectRequest();
            string ObjectName = PdsName + "\n" + SelectedEntity.DisplayShortPath;
            string Description = Repair.Description;
            string Dates = "";
            string Segment = phelper.TurnOffSegments();
            string GasVolunme = Repair.BleedAmount.ToString();
            string Plan = Repair.GazpromPlanID;
            string Potrebitel = Repair.ConsumersState;
            string Comment = Repair.DescriptionGtp;
            string performer = UserProfile.Current.UserName;
            string performer_phone = "";

            if (UserProfile.Current.Site.IsEnterprise)
            {
                PdsName = UserProfile.Current.Site.Name;
            }
            else
            {
                LpuName = UserProfile.Current.Site.Name;
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
            }

            _Signers = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)SelectedEntity.EntityType, FromId = UserProfile.Current.Site.Id, IsCpdd = false, ToId = PdsId });
            
            DateTime RepairStartDate = Repair.DateStartSched.HasValue ? Repair.DateStartSched.Value : Repair.StartDate;
            DateTime RepairEndDate = Repair.DateEndSched.HasValue ? Repair.DateEndSched.Value : Repair.EndDate;
            if (Repair.WFWState.IsProlongation)
            {
                Dates = string.Format("c {0} {1}:{2}\nпоДАТА/ВРЕМЯ\n\nпродолжительность: ... ч.\n", RepairEndDate.ToShortDateString(), RepairEndDate.Hour, RepairEndDate.Minute);
            }
            else
            {
                Dates = string.Format("c {0} {1}:{2}\nпо\n{3} {4}:{5}\nпродолжительность: {6} ч.\n", RepairStartDate.ToShortDateString(), RepairStartDate.Hour, RepairStartDate.Minute, RepairEndDate.ToShortDateString(), RepairEndDate.Hour, RepairEndDate.Minute, ((TimeSpan)(RepairEndDate - RepairStartDate)).TotalHours);
            }

            if (!String.IsNullOrWhiteSpace(Comment))
                Comment = "*Примечание: "+Comment;
                     
            try
            {
                var cusers = await new UserManagementServiceProxy().GetUsersAsync();
                performer_phone = cusers.First(o => o.Login == UserProfile.Current.Login).Phone;
            }
            catch
            {
            }

            dicParams.Add("HEADER_LPU_NAME", LpuName);
            dicParams.Add("HEADER_PDS_NAME", PdsName);
            dicParams.Add("HEADER_DOC_NAME", "Факсимильное сообщение");
            dicParams.Add("HEADER_DOC_DATE", FaxDate);
            dicParams.Add("SIGNERS_POS", targetPos);
            dicParams.Add("SIGNERS_NAME", targetName);
            dicParams.Add("HEADER_SUBJECT", Subject);
            dicParams.Add("MAIN_NUM", "1");
            dicParams.Add("MAIN_OBJECT_NAME", ObjectName);
            dicParams.Add("MAIN_DESCRIPTION", Description);
            dicParams.Add("MAIN_DATES", Dates);
            dicParams.Add("MAIN_SEGMENT", Segment);
            dicParams.Add("MAIN_GAS_VOLUME", GasVolunme);
            dicParams.Add("MAIN_PLAN", Plan);
            dicParams.Add("MAIN_POTREBITEL", Potrebitel);
            dicParams.Add("MAIN_COMMENT", Comment);
            dicParams.Add("PERFORMER_NAME", performer);
            dicParams.Add("PERFORMER_PHONE", performer_phone);

            return dicParams;
        }

        protected override async Task PerformAdditionalEditing(RadDocument CurrentDocument, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        {
            string sign_placeholder = " _____________________ ";
            string sign_agreed =      "      согласовано      ";
            Dictionary<string, List<string[]>> PersonsTablesContext = new Dictionary<string, List<string[]>>();
            List<string[]> TargetUsers = _Signers.To.Select(item => new string[2] {item.Position, item.FIO }).ToList();
            List<string[]> Signers = _Signers.From.Select(item => new string[2] { item.Position, AgreedList.Any(al_item=>(al_item.AgreedUserName== item.FIO || al_item.RealAgreedUserName == item.FIO)&& al_item.AgreedResult.GetValueOrDefault(false))? sign_agreed : sign_placeholder + item.FIO }).ToList();
            PersonsTablesContext.Add("TargetUsers", TargetUsers);
            PersonsTablesContext.Add("Signers", Signers);

            AddPersonsTables(CurrentDocument, PersonsTablesContext);
        }
    }
}
