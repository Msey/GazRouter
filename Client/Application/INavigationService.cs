using System;

namespace GazRouter.Application
{
    public interface INavigationService
    {
        void Navigate(Uri uri);
    }
}