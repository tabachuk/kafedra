using KafedraApp.ViewModels;
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
	}
}
