namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{

    /// <summary>
    /// Базовый класс для всех элементов, положение которых определяется только одной координатой
    /// </summary>
    
    public class BoxedElementModel : ElementModelBase
    {
        public BoxedElementModel()
        {
            Width = 150;
            Height = 150;
            IsBoxVisible = true;
        }


        public bool IsBoxVisible { get; set; }


        public override void CopyStyle(ElementModelBase other)
        {
            var e = other as BoxedElementModel;
            if (e != null)
                IsBoxVisible = e.IsBoxVisible;

            base.CopyStyle(other);
        }
    }

}