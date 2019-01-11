using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Common;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.Attachments;
using GazRouter.DataProviders.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.ManualInput.PipelineLimits;
using System.Threading.Tasks;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.SeriesData.GasInPipes;
using System;
using GazRouter.Application;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.Modes.ProcessMonitoring.Reports.Forms;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.PhisicalTypes;

namespace GazRouter.ManualInput.PipelineLimits
{
    public class PipelineLimitsViewModel : FormViewModelBase
    {
        public DelegateCommand AddLimitCommand { get; private set; }
        public DelegateCommand RemoveLimitCommand { get; private set; }
        public DelegateCommand EditLimitCommand { get; private set; }
        public DelegateCommand AddAttachmentCommand { get; private set; }
        public DelegateCommand<AttachmentBaseDTO> DeleteAttachmentCommand { get; private set; }

        public DelegateCommand RefreshCommand { get; private set; }
        public PipelineLimitsViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.PipelineLimits);

            _selectedSystem = SystemList.First();

            RefreshCommand = new DelegateCommand(Refresh);
            AddLimitCommand = new DelegateCommand(AddLimit, () => editPermission);
            RemoveLimitCommand = new DelegateCommand(RemoveLimit, () => SelectedItem is LimitItem && editPermission);
            EditLimitCommand = new DelegateCommand(EditLimit, () => SelectedItem is LimitItem && editPermission);
            AddAttachmentCommand = new DelegateCommand(AddAttachment, () => SelectedItem is LimitItem && editPermission);
            DeleteAttachmentCommand = new DelegateCommand<AttachmentBaseDTO>(DeleteAttachment);

            Refresh();
        }
        private void RefreshCommands()
        {
            AddLimitCommand.RaiseCanExecuteChanged();
            RemoveLimitCommand.RaiseCanExecuteChanged();
            EditLimitCommand.RaiseCanExecuteChanged();
            AddAttachmentCommand.RaiseCanExecuteChanged();
        }

        public List<GasTransportSystemDTO> SystemList
        {
            get { return ClientCache.DictionaryRepository.GasTransportSystems; }
        }
        public List<ItemBase> Items { get; set; }

        private ItemBase _selectedItem;
        private GasTransportSystemDTO _selectedSystem;

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    RefreshCommands();
            }
        }
        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (SetProperty(ref _selectedSystem, value))
                {
                    Refresh();
                }
            }
        }
        public override async void Refresh()
        {
            if (SelectedSystem == null) return;

            try
            {
                Behavior.TryLock();

                // Список газопроводов
                var pipelines = await new ObjectModelServiceProxy().GetPipelineListAsync(
                    new GetPipelineListParameterSet { SystemId = SelectedSystem.Id });

                Items = new List<ItemBase>();

                // Группы по типам газопроводов
                PipelineType[] typeList = { PipelineType.Main, PipelineType.Looping, PipelineType.Distribution };

                // todo: Добавить тип прочее

                foreach (var type in typeList)
                {
                    var pipeTypeItem = new PipeTypeItem
                    {
                        Name = ClientCache.DictionaryRepository.PipelineTypes[type].Name
                    };

                    // Группы по газопроводам
                    foreach (var pipe in pipelines.Where(p => p.Type == type))
                    {
                        var pipeItem = new PipeItem(pipe);
                        var limitList = await new ManualInputServiceProxy().GetPipelineLimitListAsync(
                            new GetPipelineLimitListParameterSet
                            {
                                PipelineId = pipe.Id
                            });
                        
                        if (limitList.Any())
                        {
                            var stationSegmentList = new List<LimitItem>();

                            foreach (var sec in limitList)
                            {
                                if (sec.Begin >= pipe.KilometerOfStartPoint && sec.End <= pipe.KilometerOfEndPoint)
                                {
                                    sec.MaxAllowableKm = pipe.KilometerOfEndPoint;
                                    sec.MinAllowableKm = pipe.KilometerOfStartPoint;
                                    stationSegmentList.Add(new LimitItem(sec));
                                }
                            }
                            if (stationSegmentList.Any())
                            {
                                pipeTypeItem.Children.Add(pipeItem);
                                pipeItem.Children.AddRange(stationSegmentList.OrderBy(s => s.KmBegin).ThenBy(s => s.KmEnd));
                            }
                        }
                        //else
                        //    pipeItem.Children.AddRange(limitList.Select(s => new LimitItem(s)));
                    }
                    if (pipeTypeItem.Children.Any())
                        Items.Add(pipeTypeItem);

                }
                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }


        private void AddLimit()
        {
            var pipelineId = SelectedItem?.PipelineId ?? Guid.Empty;
            var viewModel =  new AddEditPipelineLimitViewModel(async id =>
            {
                await new ManualInputServiceProxy().AddPipelineLimitStoryAsync(new AddPipelineLimitStoryParameterSet
                {
                    EntityId = id,
                    ChangeDate = DateTime.Now,
                    Status = LimitStatus.Active,
                    UserName = UserProfile.Current.Login
                });
                Refresh();
            }, pipelineId);
            var view = new AddEditPipelineLimitView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void EditLimit()
        {
            var viewModel = new AddEditPipelineLimitViewModel(async id =>
            {
                var Id = ((LimitItem)SelectedItem).Dto.Id;
                await new ManualInputServiceProxy().AddPipelineLimitStoryAsync(new AddPipelineLimitStoryParameterSet
                {
                    EntityId = Id,
                    ChangeDate = DateTime.Now,
                    Status = LimitStatus.Active,
                    UserName = UserProfile.Current.Login
                });
                await RefreshTree(false, Id);
            }, ((LimitItem)SelectedItem).Dto);
            var view = new AddEditPipelineLimitView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void RemoveLimit()
        {
            RadWindow.Confirm(new DialogParameters
            {
                Header = "Снятие ограничения",
                Content = new TextBlock
                {
                    Text = "Снять ограничение давление на участке? Необходимо Ваше подтверждение.",
                    TextWrapping = TextWrapping.Wrap,
                    Width = 250
                },
                OkButtonContent = "Снять",
                CancelButtonContent = "Отмена",
                Closed = async (obj, args) =>
                {
                    if (args.DialogResult.HasValue && args.DialogResult.Value)
                    {
                        var id = ((LimitItem)(SelectedItem)).Dto.Id;
                        await new ManualInputServiceProxy().DeletePipelineLimitAsync(id);

                        Refresh();
                    }
                }
            });
        }


        public async Task RefreshTree(bool IsAdd = false, int? id = null)
        {
            var limit = await new ManualInputServiceProxy().GetPipelineLimitListAsync(
                        new GetPipelineLimitListParameterSet
                        {
                            LimitId = id
                        });
            if (!IsAdd)
                EditItem(new LimitItem(limit.First()));

            var tmp = Items;
            Items = new List<ItemBase>();
            Items.AddRange(tmp);
            OnPropertyChanged(() => SelectedItem);
            OnPropertyChanged(() => Items);
        }

        public void EditItem(LimitItem item)
        {
            foreach (var pipetype in Items)
                foreach (var pipe in pipetype.Children)
                    if (pipe.PipelineId == item.PipelineId)
                    {
                        pipe.Children[pipe.Children.IndexOf(pipe.Children.FirstOrDefault(c => ((LimitItem)c).Dto.Id == item.Dto.Id))] = item;
                        SelectedItem = item ?? pipe;
                        return;
                    }
        }

        private void DeleteAttachment(AttachmentBaseDTO dto)
        {
            var d = dto as AttachmentDTO<int, int>;
            if (d != null)
            {
                RadWindow.Confirm(new DialogParameters
                {
                    Header = "Подтверждение",
                    Content = new TextBlock
                    {
                        Text = "Внимание! Удаляем прикрепленный документ. Необходимо Ваше подтверждение.",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    OkButtonContent = "Удалить",
                    CancelButtonContent = "Отмена",
                    Closed = async (obj, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            await new ManualInputServiceProxy().RemovePipelineLimitAttachmentAsync(d.Id);
                            await RefreshTree(false, d.ExternalId);
                        }
                    }
                });

            }

        }


        private void AddAttachment()
        {
            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = (AddEditAttachmentViewModel)obj;
                if (x.DialogResult.HasValue && x.DialogResult.Value)
                {
                    var select_id = ((LimitItem)SelectedItem).Dto.Id;
                    await new ManualInputServiceProxy().AddPipelineLimitAttachmentAsync(
                        new AddAttachmentParameterSet<int>
                        {
                            Description = x.Description,
                            Data = x.FileData,
                            FileName = x.FileName,
                            ExternalId = select_id
                        });
                    await RefreshTree(false, select_id);
                }
            });
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }


    }


    public abstract class ItemBase
    {
        protected ItemBase()
        {
            Children = new List<ItemBase>();
        }

        [Display(AutoGenerateField = false)]
        public List<ItemBase> Children { get; set; }

        /// <summary>
        /// Наименование объекта
        /// </summary>
        public virtual string Name { get; set; }



        public virtual string ImageSource { get; set; }


        public bool HasImage => !string.IsNullOrEmpty(ImageSource);


        /// <summary>
        /// Километр начала (участка или газопровода)
        /// </summary>
        public virtual double? KmBegin { get; set; }


        /// <summary>
        /// Километр конца (участка или газопровода)
        /// </summary>
        public virtual double? KmEnd { get; set; }

        /// <summary>
        /// Длина участка
        /// </summary>
        public virtual double? Length { get; set; }

        /// <summary>
        /// Запас газа, тыс.м3
        /// </summary>
        public virtual double? Pressure { get; set; }


        /// <summary>
        /// Изменение запаса, тыс.м3
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Запас газа, тыс.м3
        /// </summary>
        public virtual List<AttachmentDTO<int, int>> AttachmentList { get; set; }


        public virtual bool IsExpanded => true;

        public virtual Guid PipelineId { get; set; }

        public virtual DateTime? ChangeDate { get; set; }

        public virtual string UserName { get; set; }

        public virtual string UserSite { get; set; }


    }



    /// <summary>
    /// Тип газопровода
    /// </summary>
    public class PipeTypeItem : ItemBase
    {
        public override string ImageSource => "/Common;component/Images/16x16/folder.png";

        public override bool IsExpanded => false;
    }

    /// <summary>
    /// Газопровод
    /// </summary>
    public class PipeItem : ItemBase
    {
        private PipelineDTO _dto;

        public PipeItem(PipelineDTO dto)
        {
            _dto = dto;
        }
        public override Guid PipelineId => _dto.Id;

        public override string Name => _dto.Name;
        
        public override double? Length => _dto.KilometerOfEndPoint - _dto.KilometerOfStartPoint;

        public override string ImageSource => "/Common;component/Images/16x16/EntityTypes/pipeline.png";

        public override bool IsExpanded => false;

    }

    /// <summary>
    /// Участок ограничения
    /// </summary>
    public class LimitItem : ItemBase
    {
        private readonly PipelineLimitDTO _dto;

        public LimitItem(PipelineLimitDTO dto)
        {
            _dto = dto;
        }
        public PipelineLimitDTO Dto => _dto;

        public override Guid PipelineId => _dto.PipelineId;

        public override string Name => $"\tУчасток газопровода: {_dto.Begin:0.#} км. - {_dto.End:0.#} км.";

        public override double? KmBegin => _dto.Begin;

        public override double? KmEnd => _dto.End;

        public override double? Length => _dto.End - _dto.Begin;

        public override double? Pressure => UserProfile.ToUserUnits(_dto.MaxPressure.Kgh, PhysicalType.Pressure);

        public override string Description => _dto.Description;

        public override DateTime? ChangeDate => _dto.ChangeDate;

        public override string UserName => _dto.UserName;

        public override string UserSite => _dto.UserSite;

        public override List<AttachmentDTO<int, int>> AttachmentList => _dto.AttachmentList;


    }
}
