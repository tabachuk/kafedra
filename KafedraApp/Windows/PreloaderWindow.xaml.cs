using KafedraApp.Helpers;
using KafedraApp.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KafedraApp.Windows
{
    public partial class PreloaderWindow : Window
    {
		#region Fields

		private static bool _isInitialized;

		#endregion

		#region Constructors

		public PreloaderWindow()
        {
            InitializeComponent();
            LocateWindowInCenter();
        }

		#endregion

		#region Methods

		private void LocateWindowInCenter()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            Left = screenWidth / 2 - Width / 2;
            Top = screenHeight / 2 - Height / 2;
        }

        private void MovePreloader(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        public override async void BeginInit()
        {
            base.BeginInit();

            if (_isInitialized)
                return;
            _isInitialized = true;

            await Task.Delay(2000);

            var watch = new Watch().Start();
            Console.WriteLine("Initialization started.");

            await Container.Resolve<IDataService>().InitAsync();

            watch.Stop("Initialization finished. Duration: ");

            new MainWindow().Show();
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == TerminalWindow.CALL_KEY)
                TerminalWindow.Instance.Show();
        }

        #endregion
    }
}
