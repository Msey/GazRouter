using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.Dictionaries.ValveTypes;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.Application.Wrappers.Entity
{
    public class ValveWrapper : EntityWrapperBase<ValveDTO>
    {
        public ValveWrapper(ValveDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("ГТС", dto.SystemName);
            AddProperty("Газопровод", dto.PipelineName);
            AddProperty("Километр установки", dto.Kilometer.ToString("0.###"));


            AddProperty("Тип", Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == dto.ValveTypeId).Name);
            AddProperty("Назначение",
                Enumerable.Single<ValvePurposeDTO>(ClientCache.DictionaryRepository.ValvePurposes, v => v.Id == (int) dto.ValvePurposeId).Name);
            AddProperty("Контрольная точка", dto.IsControlPoint ? "да" : "нет");


            AddProperty("Диаметр байпаса 1",
                dto.Bypass1TypeId.HasValue
                    ? Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == _dto.Bypass1TypeId.Value).Name
                    : "Нет байпаса");

            AddProperty("Диаметр байпаса 2",
                dto.Bypass2TypeId.HasValue
                    ? Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == _dto.Bypass2TypeId.Value).Name
                    : "Нет байпаса");

            AddProperty("Диаметр байпаса 3",
                dto.Bypass3TypeId.HasValue
                    ? Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == _dto.Bypass3TypeId.Value).Name
                    : "Нет байпаса");
        }

        [Browsable(false)]
        public ValvePurpose ValvePurposeId
        {
            get { return _dto.ValvePurposeId; }
        }

        [Display(Name = "Тип", Order = 10)]
        public string ValveTypeName
        {
            get
            {
                return Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == _dto.ValveTypeId).Name;
            }
        }

        [Display(Name = "Назначение", Order = 20)]
        public string ValvePurposeName
        {
            get
            {
                return Enumerable.Single<ValvePurposeDTO>(ClientCache.DictionaryRepository.ValvePurposes, v => v.Id == (int)_dto.ValvePurposeId).Name;
            }
        }

        [Browsable(false)]
        public int ValveTypeId
        {
            get { return _dto.ValveTypeId; }
        }

        [Browsable(false)]
        public Guid? CompressorShopId
        {
            get { return _dto.CompShopId; }
        }

        [Display(Name = "Километр установки", Order = 30)]
        public double KilometerOfStartPoint
        {
            get { return _dto.Kilometer; }
        }

        [Display(Name = "Диаметр байпаса 1", Order = 40)]
        public string BypassDiameter1
        {
            get 
            { 
                return _dto.Bypass1TypeId.HasValue ? 
                    Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == _dto.Bypass1TypeId.Value).Name
                    : "Нет байпаса"; 
            }
        }

        [Display(Name = "Диаметр байпаса 2", Order = 50)]
        public string BypassDiameter2
        {
            get
            {
                return _dto.Bypass2TypeId.HasValue ?
                    Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == _dto.Bypass2TypeId.Value).Name
                    : "Нет байпаса";
            }
        }

        [Display(Name = "Диаметр байпаса 3", Order = 60)]
        public string BypassDiameter3
        {
            get
            {
                return _dto.Bypass3TypeId.HasValue ?
                    Enumerable.Single<ValveTypeDTO>(ClientCache.DictionaryRepository.ValveTypes, v => v.Id == _dto.Bypass3TypeId.Value).Name
                    : "Нет байпаса";
            }
        }
    }
}