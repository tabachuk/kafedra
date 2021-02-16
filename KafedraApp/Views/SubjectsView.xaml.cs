using KafedraApp.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace KafedraApp.Views
{
	public partial class SubjectsView
	{
		private SubjectsViewModel ViewModel => DataContext as SubjectsViewModel;

		public SubjectsView()
		{
			InitializeComponent();
			DataContext = new SubjectsViewModel();

			ViewModel.Subjects.CollectionChanged += OnSubjectsCollectionChanged;
			ViewModel.PropertyChanged += OnViewModelPropertyChanged;

			RefreshExtraHeaderColumnWidth();
		}

		private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ViewModel.Subjects))
				RefreshExtraHeaderColumnWidth();
		}

		private void OnSubjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshExtraHeaderColumnWidth();
		}

		private void RefreshExtraHeaderColumnWidth()
		{
			if (SubjectsSV.ComputedVerticalScrollBarVisibility == Visibility.Visible)
			{
				ExtraHeaderColumn.Width = new GridLength(SystemParameters.VerticalScrollBarWidth);
			}
			else
			{
				ExtraHeaderColumn.Width = new GridLength(0);
			}
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
