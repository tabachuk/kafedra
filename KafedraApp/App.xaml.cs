using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using KafedraApp.Properties;
using KafedraApp.Services;
using KafedraApp.Helpers;

namespace KafedraApp
{
	public partial class App : Application
	{
		#region Constants

		private const int SW_RESTORE = 9;

		#endregion

		#region Properties

		public static App Instance { get; private set; }

		public ResourceDictionary ThemeDictionary => Resources.MergedDictionaries[0];

		public string SelectedTheme
		{
			get => Settings.Default.SelectedTheme;
			private set
			{
				Settings.Default.SelectedTheme = value;
				ThemeChanged?.Invoke(this, null);
			}
		}

		#endregion

		#region Events

		public event EventHandler ThemeChanged;

		#endregion

		#region DLL Imports

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd);

		#endregion

		#region Constructors

		public App()
		{
			ShutDownIfAlreadyRuns();

#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
				CatchUnhandledException((Exception)e.ExceptionObject);

			Dispatcher.UnhandledException += (s, e) =>
				CatchUnhandledException(e.Exception);

			TaskScheduler.UnobservedTaskException += (s, e) =>
				CatchUnhandledException(e.Exception);
#endif

			Instance = this;
			InitializeComponent();
			RegisterSingletons();

			SetTheme(Settings.Default.SelectedTheme);
		}

		#endregion

		#region Methods

#if !DEBUG
		private void CatchUnhandledException(Exception exc)
		{
			Container.Resolve<IDialogService>().ShowErrorMessage(exc.Message);
		}
#endif

		public void SetTheme(string themeName)
		{
			if (string.IsNullOrWhiteSpace(themeName))
				return;

			var oldTheme = ThemeDictionary.MergedDictionaries[0];
			ThemeDictionary.MergedDictionaries.Clear();

			try
			{
				var newTheme = new ResourceDictionary
				{
					Source = new Uri($"Styles\\{ themeName }Theme.xaml", UriKind.Relative)
				};

				ThemeDictionary.MergedDictionaries.Add(newTheme);
				SelectedTheme = themeName;
			}
			catch (IOException ex)
			{
				ThemeDictionary.MergedDictionaries.Add(oldTheme);
				Console.WriteLine(ex.Message);
			}
		}

		public static void ShutDownIfAlreadyRuns()
		{
			var currentProcess = Process.GetCurrentProcess();
			var processes = Process.GetProcessesByName(currentProcess.ProcessName);

			if (processes.Length > 1)
			{
				int n = 0;

				if (processes[0].Id == currentProcess.Id)
				{
					n = 1;
				}

				var hWnd = processes[n].MainWindowHandle;

				if (IsIconic(hWnd))
				{
					ShowWindowAsync(hWnd, SW_RESTORE);
				}

				SetForegroundWindow(hWnd);
				Current.Shutdown();
			}
		}

		private void RegisterSingletons()
		{
			Container.RegisterSingleton<IDataService, DataService>();
			Container.RegisterSingleton<IDialogService, DialogService>();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			Settings.Default.Save();
			base.OnExit(e);
		}

		#endregion
	}
}
