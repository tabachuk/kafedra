using KafedraApp.ViewModels;
using System.Windows;

namespace KafedraApp.Views
{
	public partial class TeachersView
	{
		public int VerticalScrollBarWidth => (int)SystemParameters.VerticalScrollBarWidth;

		public TeachersView()
		{
			InitializeComponent();
			DataContext = new TeachersViewModel();
		}
	}
}
