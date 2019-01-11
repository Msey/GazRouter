
namespace GazRouter.DTO.DispatcherTasks
{
    public class AddTaskCpddParameterSet
	{
		public string Subject { get; set; }
        public string Description { get; set; }
        public string GlobalTaskId { get; set; }
        public string UserNameCpdd { get; set; }
		public bool IsAproved { get; set; }
	}
}
