using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.Flobus.Interfaces;

namespace GazRouter.Flobus.VM.Model
{
    /// <summary>
    ///     ����� ��� �������� ����������� ����������� � ������� �����������
    /// </summary>
    public class PipelineConnection : PipelineElementBase<PipelineConnDTO, int>, IPipelineConnectionHint
    {
        private readonly PipelineConnDTO _dto;

        public PipelineConnection(PipelineConnDTO pipelineConnDto, Pipeline pipe, Pipeline connectedPipe)
            : base(pipelineConnDto, pipe)
        {
            _dto = pipelineConnDto;

            Pipe = pipe;
            ConnectedPipeline = connectedPipe;

            ConnectedPipeline.PropertyChanged += OnPipelinePropertyChanged;

            Pipe.PropertyChanged += OnPipelinePropertyChanged;
        }

        /// <summary>
        ///     ��������� ����� ����� ����������� ���������� � �����������
        /// </summary>
        public PipelineEndType EType => _dto.EndTypeId;

        public Point? PiplineEndPosition
        {
            get
            {
                switch (EType)
                {
                    case PipelineEndType.StartType:
                        return ConnectedPipeline.StartPoint;

                    case PipelineEndType.EndType:
                        return ConnectedPipeline.EndPoint;
                }

                return null;
            }
        }

        /// <summary>
        ///     ���� ����� ����������� ��� �����������
        /// </summary>
        public Color ConnectionPointColor => Pipe.Color;

        /// <summary>
        ///     ����������� ���� ����������� ��� �����������
        /// </summary>
        public Color StandardConnectionPointColor => Pipe.StandardColor;

        /// <summary>
        ///     ������ ����� ����������� ��� �����������
        /// </summary>
        public double ConnectionPointRadius => ConnectedPipeline.Thickness/2;

        /// <summary>
        ///     ����������, � �������� �������������� �����������
        /// </summary>
        public Pipeline ConnectedPipeline { get; }

        public override double Km => Dto.Kilometr ?? double.NaN;

        public Guid DestinationPipileneId => ConnectedPipeline.Id;
        public bool ConnectToStart => EType == PipelineEndType.StartType;

        private void OnPipelinePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            NotifyNeedRefresh();
        }
    }
}