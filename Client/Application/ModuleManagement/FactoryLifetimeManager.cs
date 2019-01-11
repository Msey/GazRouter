using System;
using Microsoft.Practices.Unity;

namespace GazRouter.Application.ModuleManagement
{
    public class FactoryLifetimeManager<T> : LifetimeManager
	{
		private readonly LifetimeManager _baseManager;
		private readonly Func<T> _factoryMethod;

		public FactoryLifetimeManager(Func<T> factoryMethod, LifetimeManager baseManager)
		{
			_factoryMethod = factoryMethod;
			_baseManager = baseManager;
		}

		public override object GetValue()
		{
			object obj = _baseManager.GetValue();
			if (obj == null)
			{
				obj = _factoryMethod();
				SetValue(obj);
			}
			return obj;
		}

		public override void RemoveValue()
		{
			_baseManager.RemoveValue();
		}

		public override void SetValue(object newValue)
		{
			_baseManager.SetValue(newValue);
		}
	}
}