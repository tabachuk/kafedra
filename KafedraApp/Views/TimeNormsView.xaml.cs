using KafedraApp.ViewModels;

namespace KafedraApp.Views
{
	public partial class TimeNormsView
	{
		public TimeNormsView()
		{
			InitializeComponent();
			DataContext = new TimeNormsViewModel();
		}
	}
}
