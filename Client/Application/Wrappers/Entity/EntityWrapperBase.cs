using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using GazRouter.Common.Cache;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;

namespace GazRouter.Application.Wrappers.Entity
{
    public class EntityWrapperBase<T> : IEntityWrapper where T : CommonEntityDTO
    {
        protected readonly T _dto;
        private List<EntityProperty> _propertyList;
        

        public EntityWrapperBase(T dto, bool displaySystem)
        {
            _dto = dto;
            
            PropertyList = new List<EntityProperty>();

            

            // displaySystem - отображать или нет системные параметры, такие как идентификатор в таблице свойств объекта.
            AddProperty("Наименование", dto.Name);
            if (displaySystem)
            {
                AddProperty("Идентификатор", dto.Id.Convert().ToString().Replace("-", "").ToUpper());
                AddProperty("Виртуальный", dto.IsVirtual ? "да" : "нет");
            }
        }

        [Display(Name = "Идентификатор")]
        public string Id
        {
            get
            { return _dto.Id.Convert().ToString().Replace("-", "").ToUpper(); }
        }

        [Display(Name = "Наименование")]
        public string Name
        {
            get { return _dto.Name; }
        }

        protected void AddProperty(string name, string value)
        {
            PropertyList.Add(
                new EntityProperty
                {
                    Name = name,
                    Value = value
                });
        }

        public List<EntityProperty> PropertyList { get; set; }

        protected static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public DelegateCommand<object> CopyToClipboardCommand { get; set; }
    }

    public class EntityProperty
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DelegateCommand CopyToClipboardCommand { get; set; }

        public EntityProperty()
        {
            CopyToClipboardCommand = new DelegateCommand(() => Clipboard.SetText(Value));
        }

    }

    public interface IEntityWrapper
    {
        List<EntityProperty> PropertyList { get; set; }
    }
}