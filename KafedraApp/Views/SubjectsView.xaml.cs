using KafedraApp.ViewModels;
using System.Collections.Specialized;
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
		}

		private void OnSubjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
			HeaderSV.ScrollToHorizontalOffset(e.HorizontalOffset);
		}
	}
}
