using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Repairs.Workflow
{
    public class WorkStateDTO
    {   
        public enum WorkflowStates
        {
            Undefined = 1,
            Draft = 2,
            Annulated = 3,
            OnAgreementPDS = 4,
            AgreedPDS = 5,
            OnAgreementCPDD = 6,
            AgreedCPDD = 7,
            RefusedPDS = 8,
            RefusedCPDD = 9,
            ProlongationRequestPDS = 10,
            ProlongationAgreedPDS = 11,
            ProlongationRequestCPDD = 12,
            ProlongationAgreedCPDD = 13,
            ProlongationRefusedPDS = 14,
            ProlongationRefusedCPDD = 15,
        }

        public bool IsProlongation
        {
            get
            {
                return
                    WFState == WorkflowStates.ProlongationAgreedCPDD ||
                    WFState == WorkflowStates.ProlongationAgreedPDS ||
                    WFState == WorkflowStates.ProlongationRefusedCPDD ||
                    WFState == WorkflowStates.ProlongationRefusedPDS ||
                    WFState == WorkflowStates.ProlongationRequestCPDD ||
                    WFState == WorkflowStates.ProlongationRequestPDS;
            }
        }

        public enum WorkStates
        {
            Undefined = 1,
            Scheduled = 2,
            Annulated = 3,
            Current = 4,
            Completed = 5,
            Deferred = 6,
            Longterm = 7
        }
        public enum Stages { Request, Current, Complited }

        public static string GetQuery(Stages stage)
        {
            switch (stage)
            {
                case Stages.Request:
                    return @" and ((
r.workflow_state = 2 or 
r.workflow_state = 3 or 
r.workflow_state = 4 or 
r.workflow_state = 5 or 
r.workflow_state = 6 or
r.workflow_state =7) 
and 
(repair_state is null or repair_state = 1)) ";
                    break;
                case Stages.Current:
                    return @" and (
r.repair_state = 2 or 
r.repair_state = 4 or 
r.repair_state = 6 or 
r.repair_state = 7 ) ";
                    break;
                case Stages.Complited:
                    return @" and r.repair_state = 5 ";
                    break;
            }
            return "";
        }

        public static string GetState(WorkflowStates s)
        {
            switch (s)
            {
                case WorkflowStates.Draft: return "Черновик";
                case WorkflowStates.Annulated: return "Анулировано";
                case WorkflowStates.OnAgreementPDS: return "На согласовании в ПДС";
                case WorkflowStates.AgreedPDS: return "Согласовано ПДС";
                case WorkflowStates.OnAgreementCPDD: return "На согласовании в ЦПДД";
                case WorkflowStates.AgreedCPDD: return "Согласовано ЦПДД";
                case WorkflowStates.RefusedPDS: return "Отклонено ПДС";
                case WorkflowStates.RefusedCPDD: return "Отклонено ЦПДД";
                case WorkflowStates.ProlongationRequestPDS: return "Запрошено продление работ в ПДС";
                case WorkflowStates.ProlongationAgreedPDS: return "Согласовано продление работ в ПДС";
                case WorkflowStates.ProlongationRequestCPDD: return "Запрошено продление работ в ЦПДД";
                case WorkflowStates.ProlongationAgreedCPDD: return "Согласовано продление работ в ЦПДД";
                case WorkflowStates.ProlongationRefusedPDS: return "Продление работ отклонено ПДС";
                case WorkflowStates.ProlongationRefusedCPDD: return "Продление работ отклонено ЦПДД";
                default: return "Не определено";
            }
        }
        
        public static string GetState(WorkStates s)
        {
            switch (s)
            {
                case WorkStates.Scheduled: return "Запланированная";
                case WorkStates.Annulated: return "Анулировано";
                case WorkStates.Current: return "Текущая";
                case WorkStates.Completed: return "Выполненная";
                case WorkStates.Deferred: return "Отложенная";
                case WorkStates.Longterm: return "Долгосрочная";
                default: return "Не определено";
            }
        }

        public static string GetTarget(WorkflowStates s)
        {
            switch (s)
            {
                case WorkflowStates.Draft: return "Установить статус черновик";
                case WorkflowStates.Annulated: return "Анулировать";
                case WorkflowStates.OnAgreementPDS: return "На согласование в ПДС";
                case WorkflowStates.AgreedPDS: return "Согласовать ПДС";
                case WorkflowStates.OnAgreementCPDD: return "На согласование в ЦПДД";
                case WorkflowStates.AgreedCPDD: return "Согласовать ЦПДД";
                case WorkflowStates.RefusedPDS: return "Отклонено ПДС";
                case WorkflowStates.RefusedCPDD: return "Отклонено ЦПДД";
                case WorkflowStates.ProlongationRequestPDS: return "Запросить продление работ в ПДС";
                case WorkflowStates.ProlongationAgreedPDS: return "Согласовать продление работ в ПДС";
                case WorkflowStates.ProlongationRequestCPDD: return "Запросить продление работ в ЦПДД";
                case WorkflowStates.ProlongationAgreedCPDD: return "Согласовать продление работ в ЦПДД";
                case WorkflowStates.ProlongationRefusedPDS: return "Отклонить продление работ ПДС";
                case WorkflowStates.ProlongationRefusedCPDD: return "Отклонить продление работ ЦПДД";
                default: return "Не определено";
            }
        }
        private bool isUpdateWF = false;

        public static string GetTarget(WorkStates s)
        {
            switch (s)
            {
                case WorkStates.Annulated: return "Анулировать";
                case WorkStates.Current: return "Начать работы";
                case WorkStates.Completed: return "Отметить как выполненное";
                case WorkStates.Deferred: return "Отметить как отложенное";
                case WorkStates.Longterm: return "Отметить как долгосрочное";
                case WorkStates.Scheduled: return "Отметить как запланированное";
                default: return "Не определено";
            }
        }


        public WorkflowStates WFState { get; set; } = WorkflowStates.Undefined;
        public WorkStates WState { get; set; } = WorkStates.Undefined;

        public string Name
        {
            get
            {
                if (isUpdateWF)
                {
                    return GetState(WFState);
                }
                else
                {
                    return GetState(WState);
                }
                //if (WState == WorkStates.Annulated && (WFState == WorkflowStates.RefusedCPDD || WFState == WorkflowStates.RefusedPDS))
                //    return GetTarget(WFState);
                //else if (WState == WorkStates.Undefined)
                //{
                //    return GetState(WFState);
                //}
                //else 
                //{
                //    return GetState(WState);
                //}
                
                return "Не определено";
            }
        }

        public string Caption
        {
            get
            {
                if (isUpdateWF)
                {
                    return GetTarget(WFState);
                }
                else
                {
                    return GetTarget(WState);
                }
                //if (WState == WorkStates.Annulated && (WFState == WorkflowStates.RefusedCPDD || WFState == WorkflowStates.RefusedPDS))
                //    return GetTarget(WFState);
                //else if (WFState == WorkflowStates.ProlongationRequestCPDD || WFState == WorkflowStates.ProlongationRequestPDS)
                //    return GetTarget(WFState);
                //else if (WState == WorkStates.Undefined)
                //    return GetTarget(WFState);
                //else
                //    return GetTarget(WState);
                 
                return "Не определено";
            }
        }

        public List<WorkStateDTO> GetTransfers(bool isPds = true)
        {
            List<WorkStateDTO> result = new List<WorkStateDTO>();

            if (WFState == WorkflowStates.Undefined && WState == WorkStates.Undefined)
            {
                result.Add(new WorkStateDTO() { WFState = WorkflowStates.Draft, WState = WorkStates.Undefined, isUpdateWF = true });
            }
            else
            {
                if (isPds)
                {
                    if (WState == WorkStates.Undefined)
                    {
                        switch (WFState)
                        {

                            case WorkflowStates.Draft:
                                result.Add(new WorkStateDTO() { WFState = WorkflowStates.OnAgreementPDS, WState = WorkStates.Undefined, isUpdateWF = true });
                                break;
                            case WorkflowStates.OnAgreementPDS:
                                result.Add(new WorkStateDTO() { WFState = WorkflowStates.AgreedPDS, WState = WorkStates.Undefined, isUpdateWF = true });
                                result.Add(new WorkStateDTO() { WFState = WorkflowStates.OnAgreementCPDD, WState = WorkStates.Undefined, isUpdateWF = true });
                                result.Add(new WorkStateDTO() { WFState = WorkflowStates.RefusedPDS, WState = WorkStates.Undefined, isUpdateWF = true });
                                break;
                            case WorkflowStates.OnAgreementCPDD:
                                result.Add(new WorkStateDTO() { WFState = WorkflowStates.RefusedCPDD, WState = WorkStates.Annulated, isUpdateWF = true });
                                result.Add(new WorkStateDTO() { WFState = WorkflowStates.AgreedCPDD, WState = WorkStates.Undefined, isUpdateWF = true });
                                break;
                            case WorkflowStates.AgreedCPDD:
                                result.Add(new WorkStateDTO() { WFState = WorkflowStates.AgreedPDS, WState = WorkStates.Undefined, isUpdateWF = true });
                                break;
                            case WorkflowStates.AgreedPDS:
                                result.Add(new WorkStateDTO() { WState = WorkStates.Scheduled, WFState = WorkflowStates.AgreedPDS, isUpdateWF = false });
                                result.Add(new WorkStateDTO() { WState = WorkStates.Current, WFState = WorkflowStates.AgreedPDS, isUpdateWF = false });
                                break;

                        }
                        return result;
                    }
                    else
                    {
                        switch (WState)
                        {
                            case WorkStates.Scheduled:
                                result.Add(new WorkStateDTO() { WState = WorkStates.Current, WFState = WFState });
                                result.Add(new WorkStateDTO() { WState = WorkStates.Deferred, WFState = WFState });
                                break;
                            case WorkStates.Deferred:
                                result.Add(new WorkStateDTO() { WState = WorkStates.Current, WFState = WFState });
                                break;
                            case WorkStates.Current:
                                result.Add(new WorkStateDTO() { WState = WorkStates.Completed, WFState = WFState });
                                result.Add(new WorkStateDTO() { WState = WorkStates.Longterm, WFState = WFState });
                                break;
                            case WorkStates.Longterm:
                                result.Add(new WorkStateDTO() { WState = WorkStates.Completed, WFState = WFState });
                                break;
                            case WorkStates.Completed:
                                result.Add(new WorkStateDTO() { WState = WorkStates.Current, WFState = WFState });
                                break;
                        }

                        if (WFState == WorkflowStates.ProlongationRequestPDS)
                        {
                            result.Add(new WorkStateDTO() { WState = WState, WFState = WorkflowStates.ProlongationAgreedPDS, isUpdateWF = true });
                            result.Add(new WorkStateDTO() { WState = WState, WFState = WorkflowStates.ProlongationRequestCPDD, isUpdateWF = true });
                            result.Add(new WorkStateDTO() { WState = WState, WFState = WorkflowStates.ProlongationRefusedPDS, isUpdateWF = true });
                        }
                        else if (WFState == WorkflowStates.ProlongationRequestCPDD)
                        {
                            result.Add(new WorkStateDTO() { WState = WState, WFState = WorkflowStates.ProlongationAgreedCPDD, isUpdateWF = true });
                            result.Add(new WorkStateDTO() { WState = WState, WFState = WorkflowStates.ProlongationRefusedCPDD, isUpdateWF = true });
                        }
                        else if (WFState == WorkflowStates.ProlongationAgreedCPDD)
                        {
                            result.Add(new WorkStateDTO() { WState = WState, WFState = WorkflowStates.ProlongationAgreedPDS, isUpdateWF = true });
                        }

                    }
                }
                else
                {
                    if (WFState == WorkflowStates.Undefined)
                    {
                        result.Add(new WorkStateDTO() { WFState = WorkflowStates.Draft, WState = WorkStates.Undefined, isUpdateWF = true });
                    }
                    else if (WFState == WorkflowStates.Draft)
                    {
                        result.Add(new WorkStateDTO() { WFState = WorkflowStates.OnAgreementPDS, WState = WorkStates.Undefined, isUpdateWF = true });
                    }
                    else if (WState == WorkStates.Current || WState == WorkStates.Longterm)
                    {
                        result.Add(new WorkStateDTO() { WState = WState, WFState = WorkflowStates.ProlongationRequestPDS, isUpdateWF = true });
                    }
                }
            }

            return result;
        }
    }
}
