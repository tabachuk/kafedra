using System;
using System.Windows;
using System.Windows.Input;
using KafedraApp.ViewModels;

namespace KafedraApp.Windows
{
	public partial class MainWindow : Window
	{
		#region Properties

		public static MainWindow Instance { get; private set; }

		public MainViewModel ViewModel => DataContext as MainViewModel;

		#endregion

		#region Constructors

		public MainWindow()
		{
			Instance = this;
			InitializeComponent();
			DataContext = new MainViewModel();
		}

		#endregion

		#region Methods

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == TerminalWindow.CALL_KEY)
				TerminalWindow.Instance.Show();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			App.Instance.Shutdown();
		}

		#endregion
	}
}
