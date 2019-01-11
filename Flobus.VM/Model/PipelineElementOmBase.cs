using System;
using GazRouter.DTO;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;
using System.Windows;

namespace GazRouter.Flobus.VM.Model
{
    public abstract class PipelineElementOmBase<T> : PipelineElementBase<T,Guid>, IPipelineOmElement, ISearchable where T : NamedDto<Guid>
    {
        private int _textAngle;
        private bool _isFound;
        private bool _hidden = false;
        private Point _container_position;

        protected PipelineElementOmBase(T dto, Pipeline pipe) : base(dto, pipe)
        {
        }

        public Point ContainerPosition
        {
            get { return _container_position; }
            set { SetProperty(ref _container_position, value); }
        }
        public int TextAngle
        {
            get { return _textAngle; }
            set { SetProperty(ref _textAngle, value); }
        }

        public bool IsFound
        {
            get { return _isFound; }
            set { SetProperty(ref _isFound, value); }
        }

        public bool Hidden
        {
            get { return _hidden; }
            set { SetProperty(ref _hidden, value); }
        }
    }
}