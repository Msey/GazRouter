namespace GazRouter.Common
{
	public interface ILockable
	{
		void Lock(string lockMessage = null);
		void Unlock();
	}
}
