using KafedraApp.Commands;
using KafedraApp.Helpers;
using KafedraApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace KafedraApp.Popups
{
	public partial class SubjectImportPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly TaskCompletionSource<bool> _tcs
			= new TaskCompletionSource<bool>();

		#endregion

		#region Properties

		public Task<bool> Result => _tcs.Task;

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			private set => SetProperty(ref _isBusy, value);
		}

		private string _firstProgressTitle;
		public string FirstProgressTitle
		{
			get => _firstProgressTitle;
			private set => SetProperty(ref _firstProgressTitle, value);
		}

		private string _secondProgressTitle;
		public string SecondProgressTitle
		{
			get => _secondProgressTitle;
			private set => SetProperty(ref _secondProgressTitle, value);
		}

		private int _firstProgressValue;
		public int FirstProgressValue
		{
			get => _firstProgressValue;
			set => SetProperty(ref _firstProgressValue, value);
		}

		private int _secondProgressValue;
		public int SecondProgressValue
		{
			get => _secondProgressValue;
			set => SetProperty(ref _secondProgressValue, value);
		}

		#endregion

		#region Commands

		public ICommand CancelCommand { get; }

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Constructors

		public SubjectImportPopup()
		{
			CancelCommand = new DelegateCommand(() => SetResult(false));

			InitializeComponent();

			ExcelReader.LoadProgressChanged += OnSubjectsLoadProgressChanged;
			ExcelReader.LoadCompleted += OnSubjectsLoadCompleted;
		}

		#endregion

		#region Methods

		private void SetResult(bool result)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			ExcelReader.LoadProgressChanged -= OnSubjectsLoadProgressChanged;
			ExcelReader.LoadCompleted -= OnSubjectsLoadCompleted;

			Dispatcher.Invoke(() =>
			{
				var anim = Resources["PopAnimation"] as Storyboard;

				anim.Completed += (o, e) =>
				{
					_tcs.SetResult(result);
					IsBusy = false;
				};

				BeginStoryboard(anim);
			});
		}

		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(storage, value))
				return false;

			storage = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}

		#endregion

		#region Event handlers

		private void OnSubjectsLoadProgressChanged(ExcelLoadProgressChangedEventArgs e)
		{
			FirstProgressTitle = $"'{ e.CurrentSheetName }' в обробці... { (int)e.CurrentSheetLoadingProgress }%";
			SecondProgressTitle = $"Всього таблиць оброблено: { e.LoadedSheets }/{ e.TotalSheets }";
			FirstProgressValue = (int)e.CurrentSheetLoadingProgress;
			SecondProgressValue = e.LoadedSheets * 100 / e.TotalSheets;
		}

		private void OnSubjectsLoadCompleted(object sender, EventArgs e)
		{
			SetResult(true);
		}

		#endregion
	}
}
