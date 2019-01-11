using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using GazRouter.Controls.Converters;
using GazRouter.DataProviders;
using GazRouter.DTO.Attachments;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Controls.Attachment
{
    public partial class AttachmentItemView : UserControl
    {
        public AttachmentItemView()
        {
            InitializeComponent();

            Img.Visibility = Visibility.Collapsed;
        }


        #region DTO
        public AttachmentBaseDTO Dto
        {
            get { return GetValue(DtoProperty) as AttachmentBaseDTO; }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty DtoProperty =
            DependencyProperty.Register(
                "Dto",
                typeof(AttachmentBaseDTO),
                typeof(AttachmentItemView),
                new PropertyMetadata(OnDtoPropertyChanged));


        private static void OnDtoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = d as AttachmentItemView;
            if (v != null && v.Dto != null)
            {
                v.DescrTxt.Text = v.Dto.Description;
                v.LinkTxt.Text = v.Dto.FileName;
                v.SizeTxt.Text = DataLengthConverter.Convert(v.Dto.DataLength);
            }
        }
        #endregion



        #region DELETE COMMAND
        public DelegateCommand<AttachmentBaseDTO> DeleteCommand
        {
            get { return GetValue(DeleteCommandProperty) as DelegateCommand<AttachmentBaseDTO>; }
            set { SetValue(DeleteCommandProperty, value); }
        }
        
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(
                "DeleteCommand",
                typeof(DelegateCommand<AttachmentBaseDTO>),
                typeof(AttachmentItemView),
                new PropertyMetadata(OnDeleteCommandPropertyChanged));


        private static void OnDeleteCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = d as AttachmentItemView;
            if (v != null)
            {
                v.BtnDelete.Visibility = v.DeleteCommand != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion




        #region SHOW IMAGE
        public bool? ShowImage 
        {
            get { return GetValue(ShowImageProperty) as bool?; }
            set { SetValue(ShowImageProperty, value); }
        }

        public static readonly DependencyProperty ShowImageProperty =
            DependencyProperty.Register(
                "ShowImage",
                typeof(bool?),
                typeof(AttachmentItemView),
                new PropertyMetadata(OnShowImagePropertyChanged));


        private static void OnShowImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = d as AttachmentItemView;
            if (v != null)
            {
                v.Img.Visibility = v.ShowImage.HasValue && v.ShowImage.Value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion



        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            if (Dto != null)
                HtmlPage.Window.Navigate(UriBuilder.GetBlobHandlerUri(Dto.BlobId), "_blank");
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (DeleteCommand != null && Dto != null)
                DeleteCommand.Execute(Dto);
        }
        
    }
}
