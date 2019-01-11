namespace DataProviders
{
	public interface ILockable
	{
		void Lock(string lockMessage = null);
		void Unlock();
	}
}
