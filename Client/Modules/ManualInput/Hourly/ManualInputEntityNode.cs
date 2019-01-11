using System.Windows.Media;
using GazRouter.Controls.Tree;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.EntityValidationStatus;

namespace GazRouter.ManualInput.Hourly
{
    public class ManualInputEntityNode : EntityNode
    {
        private EntityValidationStatus _status;

        public EntityValidationStatus Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged(() => Status);
                OnPropertyChanged(() => StatusImageSource);
            }
        }


        public ManualInputEntityNode(CommonEntityDTO entity, EntityValidationStatus status) : base(entity)
        {
            Status = status;
            IsExpanded = true;
            SortOrder = entity.SortOrder;
        }

        public ImageSource StatusImageSource
        {
            get
            {
                switch (Status)
                {
                    case EntityValidationStatus.Good:
                        return
                            (ImageSource)
                                new ImageSourceConverter().ConvertFromString(
                                    @"/Common;component/Images/16x16/ok.png");

                    case EntityValidationStatus.Alarm:
                        return
                            (ImageSource)
                                new ImageSourceConverter().ConvertFromString(
                                    @"/Common;component/Images/10x10/warning_orange.png");

                    case EntityValidationStatus.Error:
                        return
                            (ImageSource)
                                new ImageSourceConverter().ConvertFromString(
                                    @"/Common;component/Images/10x10/warning.png");

                    default:
                        return null;
                }
            }
        }
    }


}