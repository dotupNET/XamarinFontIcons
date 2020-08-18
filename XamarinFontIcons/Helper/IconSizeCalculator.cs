using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamarinFontIcons
{
	public class IconSizeCalculator
	{
		public event Action<ListViewItemInfo> ItemInfoChanged;

		private ListViewItemInfo itemInfo;
		private DisplayViewInfo displayViewInfo;

		public double ScaleFactor { get; }

		public IconSizeCalculator() :
			this(-1)
		{ }

		public IconSizeCalculator(int itemsPerRow)
		{
			// We need this to get the current orientation in GetItemsPerRowDefault()
			this.displayViewInfo = new DisplayViewInfo()
			{
				DisplayInfo = DeviceDisplay.MainDisplayInfo
			};

			this.itemInfo = new ListViewItemInfo
			{
				ItemsPerRow = itemsPerRow > 0 ? itemsPerRow : GetItemsPerRowDefault()
			};

			this.ScaleFactor = 0.6;
		}

		public void ChangeDisplayViewInfo(DisplayViewInfo info)
		{
			this.displayViewInfo = info;
			this.OnItemInfoChanged();
		}

		public void SetItemsPerRow(int value)
		{
			this.itemInfo.ItemsPerRow = value;
			this.OnItemInfoChanged();
		}

		internal void IncItemsPerRow()
		{
			this.itemInfo.ItemsPerRow += 1;
			this.OnItemInfoChanged();
		}

		internal void DecItemsPerRow()
		{
			this.itemInfo.ItemsPerRow -= 1;
			this.OnItemInfoChanged();
		}

		public int GetItemsPerRowDefault()
		{
			var di = this.displayViewInfo.DisplayInfo;
			var portrait = di.Height > di.Width;

			Debug.WriteLine($"portrait: {portrait} | H:{di.Height} | W: {di.Width}");

			switch (Device.Idiom)
			{
				case TargetIdiom.Unsupported:
				case TargetIdiom.Phone:
					return portrait ? 2 : 3;

				case TargetIdiom.Tablet:
					return di.Orientation == DisplayOrientation.Portrait ? 3 : 5;

				case TargetIdiom.Desktop:
				case TargetIdiom.TV:

					return di.Orientation == DisplayOrientation.Portrait ? 3 : 5;

				case TargetIdiom.Watch:
					return 1;

			}

			return 3;
		}

		internal void OnItemInfoChanged()
		{
			var itemsPerRow = this.GetItemsPerRowDefault(); // this.itemInfo.ItemsPerRow;
			var width = this.displayViewInfo.PageWidth;
			var height = this.displayViewInfo.PageHeight;
			// / this.itemInfo.DisplayInfo.Density;
			//var fontSize = width / itemsPerRow * this.ScaleFactor;

			//this.itemInfo = new ListViewItemInfo()
			//{
			//	FontSize = fontSize,
			//	ItemsPerRow = itemsPerRow,
			//	ItemSize = width / itemsPerRow
			//};

			var lineHeight = Device.OnPlatform(1.2, 1.2, 1.3);//TODO: Change this to Device.RuntimePlatform
			const double charWidth = 1;

			var fontSize = Math.Sqrt(width * height / (lineHeight * charWidth));

			this.itemInfo.FontSize = fontSize / itemsPerRow * this.ScaleFactor;
			this.itemInfo.ItemsPerRow = itemsPerRow;
			this.ItemInfoChanged?.Invoke(this.itemInfo);
		}

		//private void ObserveOrientation()
		//{
		//	Observable.FromEventPattern<OrientationSensorChangedEventArgs>(
		//			ev => OrientationSensor.ReadingChanged += ev,
		//			ev => OrientationSensor.ReadingChanged -= ev
		//		)
		//		.Throttle(TimeSpan.FromMilliseconds(500))
		//		.Subscribe(x =>
		//		{
		//			Debug.WriteLine((x.EventArgs.Reading.Orientation.));
		//		});

		//	OrientationSensor.Start(SensorSpeed.UI);
		//}
	}
}
