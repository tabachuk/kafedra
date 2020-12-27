using KafedraApp.ViewModels;

namespace KafedraApp.Views
{
	public partial class HelpView
	{
		public HelpView()
		{
			InitializeComponent();
			DataContext = new HelpViewModel();
		}
	}
}
