using System;
using System.Windows;

namespace GazRouter.Flobus.Interfaces
{
    public interface ICompressorShop
    {
        Point Position { get; set; }
        string Name { get; }
        Guid Id { get; }
        string ShortPath { get;  }
        object Data { get; set; }
    }
}