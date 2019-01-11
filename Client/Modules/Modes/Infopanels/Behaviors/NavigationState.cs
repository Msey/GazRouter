namespace GazRouter.Modes.Infopanels.Behaviors
{
    internal class NavigationState
    {
        public NavigationState()
        {
            _state = 0;
        }

        private int _state;

        public void Activate()
        {
            _state = 1;
        }
        public void Deactivate()
        {
            _state = 0;
        }

        public bool IsInProgress()
        {
            return _state == 1;
        }
    }
}