using KafedraApp.ViewModels;

namespace KafedraApp.Views
{
	public partial class AcademicStatusesView
	{
		public AcademicStatusesView()
		{
			InitializeComponent();
			DataContext = new AcademicStatusesViewModel();
		}
	}
}
