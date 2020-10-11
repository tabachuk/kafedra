using System;
using System.Collections.Generic;

namespace KafedraApp.Helpers
{
	public static class Container
	{
		private static readonly Dictionary<Type, object> _types = new Dictionary<Type, object>();

		public static bool IsRegistered<T>() => _types.ContainsKey(typeof(T));

		public static T Resolve<T>() => (T)_types[typeof(T)];

		public static void RegisterSingleton<TInterface, TImplementation>(bool overwrite = true)
			where TInterface : class where TImplementation : TInterface, new()
		{
			if (_types.ContainsKey(typeof(TInterface)))
			{
				if (overwrite)
				{
					_types[typeof(TInterface)] = new TImplementation();
				}
			}
			else
			{
				_types.Add(typeof(TInterface), new TImplementation());
			}
		}

		public static void RegisterSingleton<T>(bool overwrite = true) where T : new()
		{
			if (_types.ContainsKey(typeof(T)))
			{
				if (overwrite)
				{
					_types[typeof(T)] = new T();
				}
			}
			else
			{
				_types.Add(typeof(T), new T());
			}
		}

		public static void RegisterInstance<T>(T instance, bool overwrite = true) where T : new()
		{
			if (_types.ContainsKey(typeof(T)))
			{
				if (overwrite)
				{
					_types[typeof(T)] = instance;
				}
			}
			else
			{
				_types.Add(typeof(T), instance);
			}
		}
	}
}
