using System;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading;
using dotup.Binding;
using dotup.Binding.Collections;
using dotup.Binding.Commands;
using NetFontIcons;

namespace XamarinFontIcons
{
	public class IconOverviewDataContext : ViewModelBase
	{
		#region Public Constructors

		public IconOverviewDataContext()
		{

			this
				.ToObservable(p => p.SearchTerm)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Select(term => term?.Trim())
				.DistinctUntilChanged()
				.Select(term => term?.ToLower(CultureInfo.CurrentCulture))
				//.Select(filterString => FilterItems(filterString))
				.ObserveOn(SynchronizationContext.Current)
				.Subscribe(result =>
					{
						if (string.IsNullOrEmpty(result))
							Icons.RemoveFilter();
						else
							Icons.Filter(icon => icon.Name.ToLower().Contains(result));
					}
				);

			// Icon size calculator
			calc = new IconSizeCalculator();
			SpanCount = calc.GetItemsPerRowDefault();

			var x = Observable.FromEvent<ListViewItemInfo>(
				evt => calc.ItemInfoChanged += evt,
				evt => calc.ItemInfoChanged -= evt
			).Subscribe(info =>
				{
					FontSize = info.FontSize;
					SpanCount = info.ItemsPerRow;
				}
			);

			var fontSet = FontIconProvider.GetAllIcons(FontFamilies.FontAwesomeSolid);
			Icons = new BindingCollection<FontIcon>(fontSet.Icons);
			//_ = Icons.Fill(fontSet.Icons);
		}

		#endregion Public Constructors

		#region Private Fields

		private readonly IconSizeCalculator calc;

		private double fontSize;

		private double itemSize;

		private string searchTerm;

		private int spanCount;

		#endregion Private Fields

		#region Public Properties

		public ObservableCommand<object> ButtonClickedCommand { get; set; }

		public double FontSize
		{
			get => fontSize;
			set => this.SetProperty(ref fontSize, value);
		}

		public BindingCollection<FontIcon> Icons { get; }

		public double ItemSize
		{
			get => itemSize;
			set => this.SetProperty(ref itemSize, value);
		}

		public string SearchTerm
		{
			get => searchTerm;
			set => this.SetProperty(ref searchTerm, value);
		}

		public int SpanCount
		{
			get => spanCount;
			set => this.SetProperty(ref spanCount, value);
		}

		#endregion Public Properties

		#region Internal Methods

		internal void ChangeItemsPerRow(bool value)
		{
			if (value)
				calc.IncItemsPerRow();
			else
				calc.DecItemsPerRow();
		}

		internal void DisplayViewInfoChanged(DisplayViewInfo currentInfo)
		{
			this.Title = currentInfo.DisplayInfo.Orientation.ToString();
			calc.ChangeDisplayViewInfo(currentInfo);
		}

		#endregion Internal Methods
	}
}