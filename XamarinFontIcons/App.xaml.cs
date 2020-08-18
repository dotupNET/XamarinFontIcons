using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinFontIcons.Views;

namespace XamarinFontIcons
{
	public partial class App : Application
	{

		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
			var content = new IconOverviewPage()
			{
				BindingContext = new IconOverviewDataContext()
			};
			MainPage.Navigation.PushAsync(content);

			//MainPage = new MainPage()
			//{
			//	Content = new IconOverview()
			//	{
			//		BindingContext = new IconOverviewDataContext()
			//	}
			//};

		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
