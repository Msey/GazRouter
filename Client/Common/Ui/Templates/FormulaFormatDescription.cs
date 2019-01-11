namespace GazRouter.Common.Ui.Templates
{
    public class FormulaFormatDescription
    {
        /// <summary> 
        /// 
        /// 1.
        ///         TopText
        /// BigText 
        ///        
        /// 2.       
        /// BigText 
        ///        SmallText
        /// 
        ///  </summary>
        /// <param name="bigText"></param>
        /// <param name="smallText"></param>
        /// <param name="up"> true - установит индекс сверху, иначе снизу </param>
        public FormulaFormatDescription(string bigText, string smallText, bool up = false)
        {
            BigText = bigText;
            if (up)
            {
                TopText = smallText;
                BottomText = " ";
            }
            else
            {
                TopText = " ";
                BottomText = smallText;
            }
        }
        /// <summary> 
        /// 
        ///         TopText
        /// BigText 
        ///         SmallText
        ///   
        /// </summary>
        /// <param name="bigText"></param>
        /// <param name="topText"></param>
        /// <param name="bottomText"></param>
        public FormulaFormatDescription(string bigText, string topText, string bottomText)
        {
            BigText = bigText;
            TopText = topText;
            BottomText = bottomText;
        }
        public string BigText { get; }
        public string TopText { get; }
        public string BottomText { get; }
    }
}
