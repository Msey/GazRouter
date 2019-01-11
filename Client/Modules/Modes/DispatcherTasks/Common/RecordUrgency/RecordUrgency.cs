namespace GazRouter.Modes.DispatcherTasks.Common.RecordUrgency
{
    public enum RecordUrgency
    {
        Default,    //нормально
        Urgent,     //приближается срок
        Alarm,      //просрочено
        NonAck      //не квитировано
    }
}