using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Reflection;

namespace XamarinFontIcons
{
	public static class Extensions
	{
		/// <summary>
		/// Returns an observable sequence of the value of a property when <paramref name="source"/> raises <seealso cref="INotifyPropertyChanged.PropertyChanged"/> for the given property.
		/// </summary>
		/// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
		/// <typeparam name="TProperty">The type of the property that is being observed.</typeparam>
		/// <param name="source">The object to observe property changes on.</param>
		/// <param name="property">An expression that describes which property to observe.</param>
		/// <returns>Returns an observable sequence of the property values as they change.</returns>
		public static IObservable<TProperty> ToObservable<T, TProperty>(this T source, Expression<Func<T, TProperty>> property)
			where T : INotifyPropertyChanged
		{
			return Observable.Create<TProperty>(o =>
			{
				var member = property.Body as MemberExpression;
				var propertyInfo = member?.Member as PropertyInfo; //.GetPropertyInfo().Name;

				if (propertyInfo == null)
					throw new ArgumentException($"Property {property.ToString()} not found");

				var propertyName = propertyInfo.Name;
				var propertySelector = property.Compile();

				return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
						handler => handler.Invoke,
						h => source.PropertyChanged += h,
						h => source.PropertyChanged -= h)
					.Where(e => e.EventArgs.PropertyName == propertyName)
					.Select(e => propertySelector(source))
					.Subscribe(o);
			});
		}
	}
}
