using KafedraApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace KafedraApp.Views
{
	public partial class LoadDistributionView
	{
		private LoadDistributionViewModel ViewModel => DataContext as LoadDistributionViewModel;

		public LoadDistributionView()
		{
			InitializeComponent();
			DataContext = new LoadDistributionViewModel();
		}

		private void OnNotDistributedLoadSVScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.VerticalChange != 0
				&& e.VerticalOffset + e.ViewportHeight > e.ExtentHeight - 20)
			{
				ViewModel.AddNotDistributedLoadItemToShow();
			}
		}

		private void OnSearchTBGotFocus(object sender, RoutedEventArgs e)
		{
			SearchTBPlaceholder.Visibility = Visibility.Collapsed;
		}

		private void OnSearchTBLostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(SearchTB.Text))
			{
				SearchTBPlaceholder.Visibility = Visibility.Visible;
			}
		}

		private void OnSearchTBTextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(SearchTB.Text))
			{
				SearchTBPlaceholder.Visibility = Visibility.Visible;
			}
			else
			{
				SearchTBPlaceholder.Visibility = Visibility.Collapsed;
			}
		}
	}
}
