using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Plan;
using System;
using System.Windows;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.Windows.Documents;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Model.Styles;

namespace GazRouter.Repair.PrintForms
{
    public abstract class FaxDocFormatterBase:IFaxDocFormatter
    {
        private readonly RepairPlanBaseDTO _Repair;
        private readonly CommonEntityDTO _SelectedEntity;
        private readonly RepairWorkList _RepairWorkList;
        private readonly List<AgreedRepairRecordDTO> _AgreedList;
        private readonly RadDocument _RDocument;
        public FaxDocFormatterBase(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        {
            _Repair = Repair;
            _SelectedEntity = SelectedEntity;
            _RepairWorkList = RepairWorkList;
            _AgreedList = AgreedList;

        }
        public FaxDocFormatterBase(RadDocument DocumentPattern, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList):
            this(Repair, SelectedEntity, RepairWorkList, AgreedList)
        {
            _RDocument = DocumentPattern;
        }

        public async Task<RadDocument> CreatePrintDocument()
        {
            RadDocument RDocument = await LoadDocPattern();
            
            if (RDocument == null)
                throw new Exception("Шаблон документа не найден");
            else
            {
                try
                {
                    Dictionary<string, string> dicParams = await GetParams(_Repair, _SelectedEntity, _RepairWorkList, _AgreedList);
                    ApplyParams(RDocument, dicParams);
                    await PerformAdditionalEditing(RDocument, _Repair, _SelectedEntity, _RepairWorkList, _AgreedList);
                    return RDocument;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Ошибка загрузки параметров.", Ex);
                }
            }
        }

        protected virtual Task PerformAdditionalEditing(RadDocument CurrentDocument, RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList)
        {
            return TaskEx.FromResult(true);
        }

        private async Task<RadDocument> LoadDocPattern()
        {
            if (_RDocument == null)
            {
                byte[] file = await LoadRemoteDocPattern();
                if (file.Length > 0)
                {
                    DocxFormatProvider DFormatProvider = new DocxFormatProvider();
                    return DFormatProvider.Import(file);
                }
            }
            return _RDocument;
        }

        protected abstract Task<byte[]> LoadRemoteDocPattern();

        protected abstract Task<Dictionary<string, string>> GetParams(RepairPlanBaseDTO Repair, CommonEntityDTO SelectedEntity, RepairWorkList RepairWorkList, List<AgreedRepairRecordDTO> AgreedList);

        private void ApplyParams(RadDocument RDocument, Dictionary<string, string> dicParams)
        {
            foreach (var param in dicParams)
                RDocument.DocumentVariables[param.Key] = param.Value;
            RDocument.FieldsDisplayMode = FieldDisplayMode.Result;
            RadDocumentEditor RDE = new RadDocumentEditor(RDocument);
            RDE.UpdateAllFields();
            foreach (Section S in RDocument.Sections)
            {
                if (S.Footers.Default != null)
                {
                    RDE = new RadDocumentEditor(S.Footers.Default.Body);
                    RDE.UpdateAllFields();
                }
            }
        }

        protected virtual void AddPersonsTables(RadDocument CurrentDocument, Dictionary<string, List<string[]>> Context)
        {
            if (CurrentDocument != null)
                foreach (string bookmark in Context.Keys)
                {
                    List<string[]> PersonInfos = Context[bookmark];
                    if (PersonInfos.Count > 0)
                    {
                        var TargetUsersBookmark = CurrentDocument.GetBookmarkByName(bookmark);
                        if (TargetUsersBookmark != null)
                        {
                            CurrentDocument.GoToBookmark(TargetUsersBookmark);
                            Span CurrentSpan = CurrentDocument.CaretPosition.GetCurrentInline() as Span;
                            StyleDefinition NewTableStyle = null;
                            if (CurrentSpan != null)
                            {
                                NewTableStyle = new StyleDefinition($"{bookmark}TableStyle", StyleType.Table);
                                NewTableStyle.BasedOnName = RadDocumentDefaultStyles.DefaultTableGridStyleName;
                                NewTableStyle.SpanProperties.FontFamily = CurrentSpan.FontFamily;
                                NewTableStyle.SpanProperties.FontSize = CurrentSpan.FontSize;
                                NewTableStyle.SpanProperties.FontStyle = CurrentSpan.FontStyle;
                                NewTableStyle.SpanProperties.FontWeight = CurrentSpan.FontWeight;
                                NewTableStyle.SpanProperties.ForeColor = CurrentSpan.ForeColor;
                                NewTableStyle.SpanProperties.HighlightColor = CurrentSpan.HighlightColor;
                                NewTableStyle.SpanProperties.UnderlineColor = CurrentSpan.UnderlineColor;
                                NewTableStyle.SpanProperties.UnderlineDecoration = CurrentSpan.UnderlineDecoration;
                                Paragraph CurrentParagraph = CurrentSpan.Parent as Paragraph;
                                if (CurrentParagraph != null)
                                {
                                    NewTableStyle.ParagraphProperties.SpacingAfter = CurrentParagraph.SpacingAfter;
                                    NewTableStyle.ParagraphProperties.SpacingBefore = CurrentParagraph.SpacingBefore;
                                    NewTableStyle.ParagraphProperties.LineSpacing = CurrentParagraph.LineSpacing;
                                }
                            }
                            CurrentDocument.StyleRepository.Add(NewTableStyle);
                            RadDocumentEditor RDE = new RadDocumentEditor(CurrentDocument);
                            Table T = new Table(Context[bookmark].Count, 2) { LayoutMode = TableLayoutMode.Fixed, StyleName = $"{bookmark}TableStyle" };
                            //T.SetGridColumnWidth(0, new TableWidthUnit(70));
                            T.SetGridColumnWidth(0, new TableWidthUnit(300));
                            T.SetGridColumnWidth(1, new TableWidthUnit(300));
                            //Paragraph P_To = new Paragraph();
                            //P_To.Inlines.Add(new Span("Кому:") { FontWeight = FontWeights.Bold, });
                            //T.Rows.First.Cells.ElementAt(0).Children.Add(P_To);
                            for (int i = 0; i < PersonInfos.Count; i++)
                            {
                                Paragraph P_Position = new Paragraph();
                                Paragraph P_FIO = new Paragraph();
                                Span S_Position = new Span(PersonInfos[i][0]);
                                Span S_FIO = new Span(PersonInfos[i][1]);
                                P_Position.Inlines.Add(S_Position);
                                P_FIO.Inlines.Add(S_FIO);
                                T.Rows.ElementAt(i).Cells.ElementAt(0).Children.Add(P_Position);
                                T.Rows.ElementAt(i).Cells.ElementAt(1).VerticalAlignment = Telerik.Windows.Documents.Layout.RadVerticalAlignment.Bottom;
                                T.Rows.ElementAt(i).Cells.ElementAt(1).Children.Add(P_FIO);
                            }
                            T.Borders = new TableBorders(new Telerik.Windows.Documents.Model.Border(BorderStyle.None));
                            RDE.InsertTable(T, false);
                            CurrentDocument.GoToBookmark(TargetUsersBookmark);
                            RDE.Delete(false);
                            if (CurrentDocument.CaretPosition.IsPositionInsideTable)
                            {
                                CurrentDocument.CaretPosition.MoveToEndOfDocumentElement(CurrentDocument.CaretPosition.GetCurrentTableBox().AssociatedTable);
                                CurrentDocument.CaretPosition.MoveToNext();
                                var Pos1 = new DocumentPosition(CurrentDocument.CaretPosition);
                                if (CurrentDocument.CaretPosition.MoveToNext())
                                {
                                    var Pos2 = new DocumentPosition(CurrentDocument.CaretPosition);
                                    CurrentDocument.Selection.AddSelectionStart(Pos1);
                                    CurrentDocument.Selection.AddSelectionEnd(Pos2);
                                }
                                RDE.Delete(false);
                            }
                        }
                    }
                }
        }
    }
}
