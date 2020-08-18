using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XamarinFontIcons
{
	public class BehaviorBase<T> : Behavior<T> where T : BindableObject
	{
		public T AssociatedObject { get; private set; }

		protected override void OnAttachedTo(T visualElement)
		{
			if (visualElement == null)
			{
				throw new ArgumentNullException(nameof(visualElement));
			}

			base.OnAttachedTo(visualElement);
			AssociatedObject = visualElement;

			if (visualElement.BindingContext != null)
			{
				BindingContext = visualElement.BindingContext;
			}

			visualElement.BindingContextChanged += OnBindingContextChanged;
		}

		protected override void OnDetachingFrom(T visualElement)
		{
			if (visualElement == null)
			{
				throw new ArgumentNullException(nameof(visualElement));
			}

			base.OnDetachingFrom(visualElement);
			visualElement.BindingContextChanged -= OnBindingContextChanged;
			AssociatedObject = null;
		}

		void OnBindingContextChanged(object sender, EventArgs e)
		{
			OnBindingContextChanged();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			BindingContext = AssociatedObject.BindingContext;
		}
	}
}
