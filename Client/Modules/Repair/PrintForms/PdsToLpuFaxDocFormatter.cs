using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.Application;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.Repairs.PrintForms;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DataProviders.Repairs;

namespace GazRouter.Repair.PrintForms
{
    public class PdsToLpuFaxDocFormatter: FaxDocFormatterBase
    {
        public PdsToLpuFaxDocFormatter(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
            : base(Repair, SelectedEntity, RepairWorkList, AgreedList)
        {
        }
        public PdsToLpuFaxDocFormatter(RadDocument DocumentPattern, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
            : base(DocumentPattern, Repair, SelectedEntity, RepairWorkList, AgreedList)
        {
        }

        protected override async Task<byte[]> LoadRemoteDocPattern()
        {
            return await new RepairsServiceProxy().GetPrintDocumentPatternAsync("PdsToLpu");
        }

        private SignersDTO _Signers;
        protected override async Task<Dictionary<string, string>> GetParams(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            PrintHelper phelper = new PrintHelper(Repair, RepairWorkList);

            string LpuHead = "...Фамилия И.О....";
            string LpuCaption = Repair.SiteName;
            string LpuPos = "Начальнику";
            string subject = phelper.GetSubjectPermit();
            string startdate = Repair.DateStartSched.HasValue ? Repair.DateStartSched.Value.ToShortDateString() : "";
            string enddate = Repair.DateEndSched.HasValue ? Repair.DateEndSched.Value.ToShortDateString() : "";
            string description = Repair.Description;
            string entityName=Repair.EntityName;
            Guid PdsId=new Guid();
            string PdsName = "";
            string siteName = Repair.SiteName;
            string performer = UserProfile.Current.UserName;
            string performer_phone = "";

            try
            {
                var cusers = await new UserManagementServiceProxy().GetUsersAsync();
                performer_phone = cusers.First(o => o.Login == UserProfile.Current.Login).Phone;

            }
            catch
            {
            }

            if (UserProfile.Current.Site.IsEnterprise)
            {
                PdsName = UserProfile.Current.Site.Name;
                PdsId = UserProfile.Current.Site.Id;
            }

            _Signers = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)SelectedEntity.EntityType, FromId = PdsId, IsCpdd = false, ToId = Repair.SiteId });
            if (_Signers.To.Count > 0)
            {
                try
                {
                    var head = _Signers.To.First(o => o.IsHead);
                    if (head != null)
                    {
                        LpuPos = head.Position;
                        LpuHead = head.FIO;
                    }
                }
                catch { }
            }
                        
            dicParams.Add("HEAD_LPU_POS", LpuPos);
            dicParams.Add("HEAD_LPU_CAPTION", LpuCaption);
            dicParams.Add("HEAD_LPU_HEAD", LpuHead);
            dicParams.Add("SUBJECT", subject);
            dicParams.Add("TEXT_START_DATE", startdate);
            dicParams.Add("TEXT_END_DATE", enddate);
            dicParams.Add("TEXT_REPAIR_DESCRIPTION", description);
            dicParams.Add("TEXT_ENTITY_NAME", entityName);
            dicParams.Add("TEXT_PDS_NAME", PdsName);

            var user = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)SelectedEntity.EntityType, FromId = Repair.SiteId, IsCpdd = false, ToId = PdsId });
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
            dicParams.Add("TEXT_LPU_POS", LpuPos);
            dicParams.Add("TEXT_SITENAME", siteName);
            dicParams.Add("TEXT_LPU_HEAD", LpuHead);

            for (int i = 0; i < _Signers.From.Count; i++)
            {
                dicParams.Add($"SIGNER{i + 1}_POS", _Signers.From[i].Position);
                dicParams.Add($"SIGNER{i + 1}_NAME", _Signers.From[i].FIO);
            }


            dicParams.Add("PERFORMER_NAME", performer);
            dicParams.Add("PERFORMER_PHONE", performer_phone);

            return dicParams;
        }

        //protected override async Task PerformAdditionalEditing(RadDocument CurrentDocument, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        //{
        //    //string sign_placeholder = " _____________________ ";
        //    //string sign_agreed = "      согласовано      ";
        //    Dictionary<string, List<string[]>> PersonsTablesContext = new Dictionary<string, List<string[]>>();
        //    //List<string[]> TargetUsers = _Signers.To.Select(item => new string[2] { item.Position, item.FIO }).ToList();
        //    //List<string[]> Signers = _Signers.From.Select(item => new string[2] { item.Position, AgreedList.Any(al_item => (al_item.AgreedUserName == item.FIO || al_item.RealAgreedUserName == item.FIO) && al_item.AgreedResult.GetValueOrDefault(false)) ? sign_agreed : sign_placeholder + item.FIO }).ToList();
        //    List<string[]> Signers = _Signers.From.Select(item => new string[2] { item.Position, item.FIO }).ToList();

        //    //PersonsTablesContext.Add("TargetUsers", TargetUsers);
        //    PersonsTablesContext.Add("Signers", Signers);

        //    AddPersonsTables(CurrentDocument, PersonsTablesContext);
        //}
    }
}
