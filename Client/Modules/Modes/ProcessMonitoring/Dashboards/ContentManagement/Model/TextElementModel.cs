using System.Windows.Media;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{

    public class TextElementModel : BoxedElementModel
    {
        public TextElementModel()
        {
            FontSize = 11;
            FontColor = Colors.Black;
            FontStyle = MyFontStyle.Normal;
        }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }
        
    }
}