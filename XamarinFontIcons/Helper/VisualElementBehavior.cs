using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamarinFontIcons
{
	public class VisualElementBehavior : BehaviorBase<VisualElement>
	{

		private DisplayViewInfo currentInfo;

		private readonly CompositeDisposable disposables;

		public VisualElementBehavior()
		{
			disposables = new CompositeDisposable();
		}

		protected override void OnAttachedTo(VisualElement visualElement)
		{
			// Set the previous display 
			currentInfo = new DisplayViewInfo
			{
				DisplayInfo = DeviceDisplay.MainDisplayInfo,
				PageHeight = 0,
				PageWidth = 0
			};

			// Size changed handler
			var sizeChangedSubscription = Observable.FromEventPattern(
					evt => visualElement.SizeChanged += evt,
					evt => visualElement.SizeChanged -= evt
				)
				.Sample(TimeSpan.FromMilliseconds(1000))
				.Select(o => new Tuple<double, double>(visualElement.Width, visualElement.Height))
				.Where(t => t.Item1 > 0 && t.Item2 > 0)
				//.DistinctUntilChanged()
				.ObserveOn(SynchronizationContext.Current)
				.Subscribe(value =>
					{
						var (item1, item2) = value;
						currentInfo.PageWidth = item1;
						currentInfo.PageHeight = item2;

						OnInfoChanged();
					}
				);
			;

			disposables.Add(sizeChangedSubscription);

			// Orientation changed handler
			var displaySubscription = Observable.FromEventPattern<DisplayInfoChangedEventArgs>(
						evt => DeviceDisplay.MainDisplayInfoChanged += evt,
						evt => DeviceDisplay.MainDisplayInfoChanged -= evt
					)
					.Select(x => x.EventArgs.DisplayInfo)
					.DistinctUntilChanged(this.DisplayInfoComparer)
					.Subscribe(info =>
					{
						currentInfo.DisplayInfo = info;
						OnInfoChanged();
					})
				;

			disposables.Add(displaySubscription);

			base.OnAttachedTo(visualElement);
		}

		protected override void OnDetachingFrom(VisualElement visualElement)
		{
			disposables.Dispose();
			base.OnDetachingFrom(visualElement);
		}

		protected virtual void OnInfoChanged()
		{
			var vm = this.BindingContext as IconOverviewDataContext;
			vm?.DisplayViewInfoChanged(currentInfo);
		}

		private int DisplayInfoComparer(DisplayInfo arg)
		{
			if (currentInfo == null) return 1;

			if (currentInfo.DisplayInfo.Orientation != arg.Orientation) return 1;

			return 0;
		}
	}
}