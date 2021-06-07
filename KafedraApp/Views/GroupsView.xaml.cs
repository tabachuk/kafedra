using KafedraApp.ViewModels;
using System.Windows;

namespace KafedraApp.Views
{
	public partial class GroupsView
	{
		public int VerticalScrollBarWidth => (int)SystemParameters.VerticalScrollBarWidth;

		public GroupsView()
		{
			InitializeComponent();
			DataContext = new GroupsViewModel();
		}
	}
}
