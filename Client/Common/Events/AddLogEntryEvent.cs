using System;
using Microsoft.Practices.Prism.Events;

namespace GazRouter.Common.Events
{
    public class AddLogEntryEvent : CompositePresentationEvent<Tuple<string, string>>
    {
    }
}