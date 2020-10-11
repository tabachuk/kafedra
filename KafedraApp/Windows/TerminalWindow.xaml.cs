using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KafedraApp.ViewModels;

namespace KafedraApp.Windows
{
	public partial class TerminalWindow : Window
	{
		#region Constants

		public const Key CALL_KEY = Key.Tab;

		#endregion

		#region Properties

		private TerminalViewModel ViewModel => DataContext as TerminalViewModel;

		private static TerminalWindow _instance;
		public static TerminalWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = new TerminalWindow();

				return _instance;
			}
		}

		#endregion

		#region Constructors

		public TerminalWindow()
		{
			InitializeComponent();
			DataContext = new TerminalViewModel();
			ViewModel.Quitted += OnQuitted;
		}

		#endregion

		#region Methods

		public async override void EndInit()
		{
			base.EndInit();
			RequestTB.IsEnabled = false;
			await Task.Delay(500);
			RequestTB.IsEnabled = true;
			RequestTB.Focus();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			RequestTB.Focus();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			_instance = null;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			switch (e.Key)
			{
				case Key.Enter:
					ViewModel.ExecuteCommand();
					OutputsSV.ScrollToBottom();
					break;
				case Key.Up:
					ViewModel.PastePreviousRequest();
					RequestTB.CaretIndex = int.MaxValue;
					break;
			}
		}

		private void OnQuitted(object sender, EventArgs e)
		{
			Close();
		}

		#endregion
	}
}
