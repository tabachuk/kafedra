using KafedraApp.Commands;
using KafedraApp.Models;
using KafedraApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Container = KafedraApp.Helpers.Container;

namespace KafedraApp.Views
{
	public partial class TimeNormView : INotifyPropertyChanged
	{
		#region Fields

		private readonly IDialogService _dialogService;

		#endregion

		#region Properties

		public TimeNorm TimeNorm => DataContext as TimeNorm;

		public double CurrentHours => TimeNorm?.Hours ?? 0;

		private bool _isHoursInEdit;
		public bool IsHoursInEdit
		{
			get => _isHoursInEdit;
			set
			{
				SetProperty(ref _isHoursInEdit, value);

				if (value)
				{
					HoursTB.Focus();
					HoursTB.CaretIndex = int.MaxValue;
				}
			}
		}

		private string _hoursInEdit;
		public string HoursInEdit
		{
			get => _hoursInEdit;
			set => SetProperty(ref _hoursInEdit, value);
		}

		#endregion

		#region Constructors

		public TimeNormView()
		{
			_dialogService = Container.Resolve<IDialogService>();

			EditHoursCommand = new DelegateCommand(EditHours);
			SaveHoursCommand = new DelegateCommand(SaveHours);
			NotSaveHoursCommand = new DelegateCommand(NotSaveHours);

			InitializeComponent();
		}

		#endregion

		#region Commands

		public ICommand EditHoursCommand { get; private set; }
		public ICommand SaveHoursCommand { get; private set; }
		public ICommand NotSaveHoursCommand { get; private set; }

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Methods

		private void EditHours()
		{
			IsHoursInEdit = true;
		}

		private async void SaveHours()
		{
			if (string.IsNullOrWhiteSpace(HoursInEdit))
			{
				await _dialogService.ShowError("Заповніть поле");
				HoursTB.Focus();
				return;
			}

			if (!double.TryParse(HoursInEdit, out double hoursInEdit)
				|| hoursInEdit < 0)
			{
				await _dialogService.ShowError("Хибне значення");
				HoursTB.Focus();
				return;
			}

			if (hoursInEdit > 2000)
			{
				await _dialogService
					.ShowError("Кількість годин не повинна перевищувати 2000");
				HoursTB.Focus();
				return;
			}

			TimeNorm.Hours = hoursInEdit;
			IsHoursInEdit = false;
		}

		private void NotSaveHours()
		{
			HoursInEdit = CurrentHours.ToString();
			IsHoursInEdit = false;
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

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property.Name == nameof(DataContext))
			{
				HoursInEdit = CurrentHours.ToString();
			}
		}

		#endregion
	}
}
