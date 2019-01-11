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
using GazRouter.DTO.Repairs.PrintForms;
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
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;

namespace GazRouter.Repair.PrintForms
{
    public class PdsToCpddFaxDocFormatter : FaxDocFormatterBase
    {
        private const string remont_name = "ремонтируется";
        public PdsToCpddFaxDocFormatter(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
            : base(Repair, SelectedEntity, RepairWorkList, AgreedList)
        {
        }

        public PdsToCpddFaxDocFormatter(RadDocument DocumentPattern, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList) 
            : base(DocumentPattern, Repair, SelectedEntity, RepairWorkList, AgreedList)
        {
        }

        protected override async Task<byte[]> LoadRemoteDocPattern()
        {
            return await new RepairsServiceProxy().GetPrintDocumentPatternAsync("PdsToCpdd");
        }

        private SignersDTO _Signers;
        protected override async Task<Dictionary<string, string>> GetParams(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            PrintHelper phelper = new PrintHelper(Repair, RepairWorkList);
            string sign_placeholder = "  _______________  ";
            _Signers = await new RepairsServiceProxy().GetSighersAsync(new DTO.Repairs.PrintForms.GetSignersSet() { EntityTypeId = (int)SelectedEntity.EntityType, ToId = UserProfile.Current.Site.Id, IsCpdd = true });

            string PdsName = UserProfile.Current.Site.IsEnterprise ? UserProfile.Current.Site.Name : "";
            string FaxDate = DateTime.Today.ToLongDateString();
            string dep1 = "";
            string dep1Name = "";
            string dep1Fax = "";
            string dep2 = "";
            string dep2Name = "";
            string dep2Fax = "";
            string Subject = phelper.GetSubjectRequest();            
            string ObjectName = $"{PdsName}\n{SelectedEntity.DisplayShortPath}";
            string diameter = await GetDiameterText(Repair, RepairWorkList);
            string Description = Repair.Description;
            string Dates = $"c {(Repair.WFWState.IsProlongation ? (Repair.DateEndSched.HasValue? Repair.DateEndSched.Value.ToShortDateString() : Repair.EndDate.ToShortDateString()) : (Repair.DateStartSched.HasValue? Repair.DateStartSched.Value.ToShortDateString() : Repair.StartDate.ToShortDateString()))}\nпо" +
                           $"\n{(Repair.WFWState.IsProlongation ? "..." : (Repair.DateEndSched.HasValue ? Repair.DateEndSched.Value.ToShortDateString() : Repair.EndDate.ToShortDateString()))}";
            string GasVolunme = Repair.BleedAmount.ToString();
            string Power = $"Факт: ---\nТВПС: {Repair.CalculatedTransfer}";
            string Section = phelper.TurnOffSegments();
            string Comment = Repair.DescriptionGtp;
            string dep2sign = "";
            string performer = UserProfile.Current.UserName;
            string performer_phone = "";

            if (!String.IsNullOrWhiteSpace(Comment))
                Comment = "*Примечание: "+Comment;

            if (_Signers.To.Count > 0)
            {
                dep1 = _Signers.To[0].Position;
                dep1Name = _Signers.To[0].FIO;
                dep1Fax = _Signers.To[0].Fax;

                if (_Signers.To.Count > 1)
                {
                    for (int i = 1; i < _Signers.To.Count; i++)
                    {
                        dep2 += _Signers.To[i].Position + "\n";
                        dep2Name += _Signers.To[i].FIO + "\n";
                        dep2Fax += _Signers.To[i].Fax + "\n";
                        dep2sign += _Signers.To[i].Position + sign_placeholder + _Signers.To[i].FIO + "\n\n";
                    }
                    dep2 = dep2.TrimEnd('\n');
                    dep2Name = dep2Name.TrimEnd('\n');
                    dep2Fax = dep2Fax.TrimEnd('\n');
                    dep2sign = dep2sign.TrimEnd('\n');
                }
            }  
                      
            try
            {
                var cusers = await new UserManagementServiceProxy().GetUsersAsync();
                performer_phone = cusers.First(o => o.Login == UserProfile.Current.Login).Phone;
            }
            catch
            {
            }

            dicParams.Add("HEADER_PDS_NAME", PdsName);
            dicParams.Add("HEADER_DOC_NAME", "ФАКСИМИЛЬНОЕ СООБЩЕНИЕ");
            dicParams.Add("HEADER_DOC_NUMBER", "№ ________________");
            dicParams.Add("HEADER_DOC_DATE", FaxDate);
            dicParams.Add("HEADER_TO_DEP", dep1);
            dicParams.Add("HEADER_TO_NAME", dep1Name);
            dicParams.Add("HEADER_TO_FAX", dep1Fax);
            if(!String.IsNullOrWhiteSpace(dep2))
            {
                dicParams.Add("HEADER_COPY_TEXT", "Копия:");
                dicParams.Add("HEADER_COPY_DEP", dep2);
                dicParams.Add("HEADER_COPY_NAME", dep2Name);
                dicParams.Add("HEADER_COPY_FAX", dep2Fax);
            }
            dicParams.Add("HEADER_SUBJECT", Subject);
            dicParams.Add("MAIN_NUM", "1");
            dicParams.Add("MAIN_OBJECT_NAME", ObjectName);
            dicParams.Add("MAIN_DIAMETER", diameter);
            dicParams.Add("MAIN_DESCRIPTION", Description);
            dicParams.Add("MAIN_DATES", Dates);
            dicParams.Add("MAIN_GAS_VOLUME", GasVolunme);
            dicParams.Add("MAIN_POWER", Power);
            dicParams.Add("MAIN_SECTION",Section);
            dicParams.Add("MAIN_COMMENT", Comment);
            //for (int i = 0; i < _Signers.From.Count; i++)
            //{
            //    dicParams.Add($"SIGNER{i + 1}_POS", _Signers.From[i].Position);
            //    dicParams.Add($"SIGNER{i + 1}_NAME", sign_placeholder + _Signers.From[i].FIO);
            //}
            dicParams.Add("DEP_SIGNERS", dep2sign);
            dicParams.Add("PERFORMER_NAME", performer);
            dicParams.Add("PERFORMER_PHONE", performer_phone);

            return dicParams;
        }

        private async Task<string> GetDiameterText(RepairPlanBaseDTO _repair, RepairWorkList _works)
        {
            string result = " - ";

            if (_repair.EntityType == EntityType.Pipeline)
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
            
            return result;
        }

        protected override async Task PerformAdditionalEditing(RadDocument CurrentDocument, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        {
            string sign_placeholder = " ________________ ";
            ///string sign_agreed =   "    согласовано   ";
            Dictionary<string, List<string[]>> PersonsTablesContext = new Dictionary<string, List<string[]>>();
            //List<string[]> TargetUsers = _Signers.To.Select(item => new string[2] { item.Position, item.FIO }).ToList();
            //List<string[]> Signers = _Signers.From.Select(item => new string[2] { item.Position, AgreedList.Any(al_item => (al_item.AgreedUserName == item.FIO || al_item.RealAgreedUserName == item.FIO) && al_item.AgreedResult.GetValueOrDefault(false)) ? sign_agreed : sign_placeholder + item.FIO }).ToList();
            List<string[]> Signers = _Signers.From.Select(item => new string[2] { item.Position, sign_placeholder + item.FIO }).ToList();
            //PersonsTablesContext.Add("TargetUsers", TargetUsers);
            PersonsTablesContext.Add("Signers", Signers);

            AddPersonsTables(CurrentDocument, PersonsTablesContext);
        }
    }
}

