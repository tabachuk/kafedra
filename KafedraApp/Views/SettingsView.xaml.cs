using KafedraApp.ViewModels;

namespace KafedraApp.Views
{
	public partial class SettingsView
	{
		public SettingsView()
		{
			InitializeComponent();
			DataContext = new SettingsViewModel();
		}
	}
}
