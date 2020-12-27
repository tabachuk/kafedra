using KafedraApp.ViewModels;

namespace KafedraApp.Views
{
	public partial class GroupsView
	{
		public GroupsView()
		{
			InitializeComponent();
			DataContext = new GroupsViewModel();
		}
	}
}
