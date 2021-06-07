using KafedraApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace KafedraApp.Views
{
	public partial class SubjectsView
	{
		private SubjectsViewModel ViewModel => DataContext as SubjectsViewModel;

		public int VerticalScrollBarWidth => (int)SystemParameters.VerticalScrollBarWidth;

		public SubjectsView()
		{
			InitializeComponent();
			DataContext = new SubjectsViewModel();
		}

		private void OnSubjectsSVScrolled(object sender, ScrollChangedEventArgs e)
		{
			if (e.HorizontalChange != 0)
			{
				HeaderSV.ScrollToHorizontalOffset(e.HorizontalOffset);
			}

			if (e.VerticalChange != 0
				&& e.VerticalOffset + e.ViewportHeight > e.ExtentHeight - 100)
			{
				ViewModel.AddSubjectToShow();
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
