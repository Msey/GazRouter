using ExcelDna.Integration.CustomUI;

namespace GR_ExcelFunc.View
{
    /// <summary>
    /// класс отвечает за создание и удаление панели задач в Excel
    /// </summary>
    internal static class TaskPaneManager
    {
        private static CustomTaskPane _customTaskPane;

        public static void ShowTaskPane()
        {
            if (_customTaskPane == null)
            {
                //_customTaskPane = CustomTaskPaneFactory.CreateCustomTaskPane(typeof(TaskPaneControl), "GazRouter task pane");
                var data = new Model.SelectObjectParameterData();
                var view = new View.GetObjectParameterControl();
                //var presnter = new Presenter.SelectObjectParameterPresenter(_customTaskPane.ContentControl as TaskPaneControl, data);
                var presnter = new Presenter.SelectObjectParameterPresenter(view, data);
                _customTaskPane = CustomTaskPaneFactory.CreateCustomTaskPane(view, "Выбор параметра объекта");


                _customTaskPane.Visible = true;
                _customTaskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionLeft;
            }
            else
            {
                _customTaskPane.Visible = true;
            }
        }

        public static void DeleteTaskPane()
        {
            if (_customTaskPane == null) return;
            _customTaskPane.Delete();
            _customTaskPane = null;
        }

    }
}
