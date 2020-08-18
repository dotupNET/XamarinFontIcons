using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace dotup.Validation
{
	public static class Validator
	{
		public static void MustNotNull(object value, [CallerMemberName] string instanceName = "")
		{
			if (value == null)
				throw new ArgumentNullException($"{instanceName} is null");
		}

		public static void MustBeType<T>(object value, [CallerMemberName] string instanceName = "")
		{
			if (!(value is T))
				throw new ArgumentException($"{instanceName} is not type {typeof(T)}");
		}

		public static void MustBeTypeAndNotNull<T>(object value, [CallerMemberName] string instanceName = "")
		{
			MustBeType<T>(value, instanceName);
			MustNotNull(value, instanceName);
		}

		public static void MustBeValidParameter<T>(object value, [CallerMemberName] string instanceName = "")
		{
			if (value == null)
			{
				// Parameter "value" is null. Is T Nullable?
				if (Nullable.GetUnderlyingType(typeof(T)) == null)
				{
					// T is not nullable
					throw new ArgumentNullException($"{instanceName} is null");
				}
			}
			else
			{
				// Parameter "value" is not null. Is value of type T?
				MustBeType<T>(value);
				return;
			}
		}

	}
}
