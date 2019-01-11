using GazRouter.DTO.Repairs.Plan;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Repairs.Workflow;

namespace GazRouter.Repair.PrintForms
{
    public class PrintHelper
    {
        RepairPlanBaseDTO _repair;
        RepairWorkList _works;

        List<string> cod_names = new List<string> { "стравливается", "отключается" };
        //int[] codes = new int[] { 81, 82, 211 };

        public PrintHelper(RepairPlanBaseDTO repair, RepairWorkList RepairWorkList)
        {
            _repair = repair;
            _works = RepairWorkList;
        }

        private string SubjectPartAction
        {
            get
            {
                if (
                    _repair.WFWState.WFState == WorkStateDTO.WorkflowStates.ProlongationRefusedCPDD ||
                    _repair.WFWState.WFState == WorkStateDTO.WorkflowStates.ProlongationRefusedPDS ||
                    _repair.WFWState.WFState == WorkStateDTO.WorkflowStates.ProlongationRequestPDS || 
                    _repair.WFWState.WFState == WorkStateDTO.WorkflowStates.ProlongationRequestCPDD)
                {
                    return "продление";
                }
                else
                {
                    return "проведение";
                }
            }
        }
        public  string GetSubjectRequest()
        {
            string res = "Запрос на {0} {1} работ";
            switch (_repair.PlanType)
            {
                //PlanType.Planned
                case 1: return string.Format(res, SubjectPartAction, "плановых");
                //PlanType.Unplanned
                case 2: return string.Format(res, SubjectPartAction, "внеплановых");
                //PlanType.Emergency
                case 3: return string.Format(res, SubjectPartAction, "аварийных"); 
                default: return string.Format(res, SubjectPartAction, "внеплановых"); 
            }
        }

        private string SubjectPartPermission
        {
            get
            {
                if (_repair.WFWState.WFState == WorkStateDTO.WorkflowStates.ProlongationRefusedCPDD || 
                    _repair.WFWState.WFState == WorkStateDTO.WorkflowStates.ProlongationRefusedPDS ||
                    _repair.WFWState.WFState ==  WorkStateDTO.WorkflowStates.RefusedCPDD ||
                    _repair.WFWState.WFState ==  WorkStateDTO.WorkflowStates.RefusedPDS)
                {
                    return "Запрет";
                }
                else
                {
                    return "Разрешение";
                }
            }
        }
        public string GetSubjectPermit()
        {
            string res = "{0} на {1} {2} работ";
            switch (_repair.PlanType)
            {
                case 1: return string.Format(res, SubjectPartPermission, SubjectPartAction, "плановых");
                //PlanType.Unplanned
                case 2: return string.Format(res, SubjectPartPermission, SubjectPartAction, "внеплановых");
                //PlanType.Emergency
                case 3: return string.Format(res, SubjectPartPermission, SubjectPartAction, "аварийных");
                default: return string.Format(res, SubjectPartPermission, SubjectPartAction, "внеплановых");
            }
            //switch (_repair.PlanType)
            //{
            //    //PlanType.Planned
            //    case 1: return "Разрешение на проведение плановых работ";
            //    //PlanType.Unplanned
            //    case 2: return "Разрешение на проведение внеплановых работ";
            //    //PlanType.Emergency
            //    case 3: return "Разрешение на проведение аварийных работ";
            //    default: return "Разрешение на проведение внеплановых работ";
            //}
        }

        public string TurnOffSegments()
        {
            string result = "";

            foreach (var work in _works)
            {
                if (work.IsSelected)
                {
                    foreach (string s in cod_names)
                    {
                        if (work.Dto.Name.ToLower().Contains(s))
                        //cod_names.Contains(work.Dto.Name.ToLower()))
                        {
                            if (work.KilometerStart == null && work.KilometerEnd == null)
                                result += string.Format("{0}\n", work.Dto.Name);
                            else
                            result += string.Format("{0} {1} - {2} км\n", work.Dto.Name.ToLower(), work.KilometerStart, work.KilometerEnd);
                            //work.Dto.SystemName
                        }
                    }
                }
            }

            return result;
        }
    }
}
