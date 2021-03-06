﻿using KafedraApp.Helpers;
using KafedraApp.Properties;
using KafedraApp.Services;
using KafedraApp.Validators;
using NLog;
using NLog.Targets;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

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

			LogConfig();

#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += async (s, e) =>
				await CatchUnhandledException((Exception)e.ExceptionObject);

			//Dispatcher.UnhandledException += async (s, e) =>
			//	await CatchUnhandledException(e.Exception);

			TaskScheduler.UnobservedTaskException += async (s, e) =>
				await CatchUnhandledException(e.Exception);
#endif

			Instance = this;
			InitializeComponent();
			RegisterSingletons();

			SetTheme(Settings.Default.SelectedTheme);
		}

		#endregion

		#region Methods

#if !DEBUG
		private async System.Threading.Tasks.Task CatchUnhandledException(Exception exc)
		{
			var dialogService = Container.Resolve<IDialogService>();

			if (dialogService?.CanShowDialog == true)
			{
				await dialogService.ShowError(exc.Message);
			}
			else
			{
				MessageBox.Show(
					exc.Message,
					"Критична помилка",
					MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}
#endif

		private void LogConfig()
		{
			var config = LogManager.Configuration;

			var nowStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");

			var fileTarget = new FileTarget
			{
				Layout = "[${longdate}] [${uppercase:${level}}] [${logger}] ${message}",
				FileName = "${basedir}\\logs\\" + nowStr + ".log"
			};

			LogManager.Configuration.AddTarget("file", fileTarget);

			var rule = LogManager.Configuration.FindRuleByName("main");
			rule.Targets.Add(fileTarget);

			LogManager.Configuration = config;
		}

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
			// Services.
			Container.RegisterSingleton<IDataService, DataService>();
			Container.RegisterSingleton<IDialogService, DialogService>();

			// Validators.
			Container.RegisterSingleton<ILoadItemValidator, LoadItemValidator>();
			Container.RegisterSingleton<ITeacherValidator, TeacherValidator>();
			Container.RegisterSingleton<IGroupValidator, GroupValidator>();
			Container.RegisterSingleton<ISubjectValidator, SubjectValidator>();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			Settings.Default.Save();
			base.OnExit(e);
		}

		#endregion
	}
}
