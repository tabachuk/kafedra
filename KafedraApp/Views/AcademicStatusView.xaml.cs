using KafedraApp.Commands;
using KafedraApp.Models;
using KafedraApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Container = KafedraApp.Helpers.Container;

namespace KafedraApp.Views
{
	public partial class AcademicStatusView : INotifyPropertyChanged
	{
		#region Fields

		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;

		#endregion

		#region Properties

		public AcademicStatusInfo AcademicStatusInfo =>
			DataContext as AcademicStatusInfo;

		public int CurrentMaxHours => AcademicStatusInfo?.MaxHours ?? 0;

		private bool _isMaxHoursInEdit;
		public bool IsMaxHoursInEdit
		{
			get => _isMaxHoursInEdit;
			set
			{
				SetProperty(ref _isMaxHoursInEdit, value);

				if (value)
				{
					MaxHoursTB.Focus();
					MaxHoursTB.CaretIndex = int.MaxValue;
				}
			}
		}

		private string _maxHoursInEdit;
		public string MaxHoursInEdit
		{
			get => _maxHoursInEdit;
			set => SetProperty(ref _maxHoursInEdit, value);
		}

		public ObservableCollection<Teacher> _teachers;
		public ObservableCollection<Teacher> Teachers
		{
			get => _teachers;
			set => SetProperty(ref _teachers, value);
		}

		#endregion

		#region Constructors

		public AcademicStatusView()
		{
			_dialogService = Container.Resolve<IDialogService>();
			_dataService = Container.Resolve<IDataService>();

			EditMaxHoursCommand = new DelegateCommand(EditMaxHours);
			SaveMaxHoursCommand = new DelegateCommand(SaveMaxHours);
			NotSaveMaxHoursCommand = new DelegateCommand(NotSaveMaxHours);

			InitializeComponent();
		}

		#endregion

		#region Commands

		public DelegateCommand EditMaxHoursCommand { get; set; }

		public DelegateCommand SaveMaxHoursCommand { get; set; }

		public DelegateCommand NotSaveMaxHoursCommand { get; set; }

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Methods

		private void EditMaxHours()
		{
			IsMaxHoursInEdit = true;
		}

		private async void SaveMaxHours()
		{
			if (string.IsNullOrWhiteSpace(MaxHoursInEdit))
			{
				await _dialogService.ShowError("Заповніть поле");
				MaxHoursTB.Focus();
				return;
			}

			if (!int.TryParse(MaxHoursInEdit, out int maxHoursInEdit)
				|| maxHoursInEdit < 1)
			{
				await _dialogService.ShowError("Хибне значення");
				MaxHoursTB.Focus();
				return;
			}

			if (maxHoursInEdit > 2000)
			{
				await _dialogService
					.ShowError("Кількість годин не повинна перевищувати 2000");
				MaxHoursTB.Focus();
				return;
			}

			AcademicStatusInfo.MaxHours = maxHoursInEdit;
			IsMaxHoursInEdit = false;
		}

		private void NotSaveMaxHours()
		{
			MaxHoursInEdit = CurrentMaxHours.ToString();
			IsMaxHoursInEdit = false;
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
				MaxHoursInEdit = CurrentMaxHours.ToString();
				Teachers = new ObservableCollection<Teacher>(_dataService.Teachers
					.Where(x => x?.AcademicStatus == AcademicStatusInfo?.Status));
			}
		}

		#endregion
	}
}
