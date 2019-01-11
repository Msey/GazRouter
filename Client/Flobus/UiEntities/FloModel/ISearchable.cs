using System;
using System.Windows;

namespace GazRouter.Flobus.UiEntities.FloModel
{
    public interface ISearchable
    {
        Guid Id { get; }
        string Name { get; }
        string ShortPath { get; }
        Point Position { get; }
        string TypeName { get; }

        bool IsFound { get;set; }
    }
}