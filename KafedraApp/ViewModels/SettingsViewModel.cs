using KafedraApp.Commands;
using KafedraApp.Helpers;
using KafedraApp.Services;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class SettingsViewModel : BindableBase
	{
		#region Fields

		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		#endregion

		#region Properties

		private string _dataPath;
		public string DataPath
		{
			get => _dataPath;
			set => SetProperty(ref _dataPath, value, OnDataPathChanged);
		}

		public bool IsDataPathCorrect => Directory.Exists(DataPath);

		public bool IsDataPathDefault => DataPath == _dataService.DefaultDataPath;

		public bool CanSaveDataPath => IsDataPathCorrect && _dataService.DataPath != DataPath;

		#endregion

		#region Commands

		public ICommand BrowseDataPathCommand { get; }
		public ICommand OpenDataFolderCommand { get; }
		public ICommand SaveDataPathCommand { get; }
		public ICommand ResetDataPathCommand { get; }

		#endregion

		#region Constructors

		public SettingsViewModel()
		{
			BrowseDataPathCommand = new DelegateCommand(BrowseDataPath);
			OpenDataFolderCommand = new DelegateCommand(OpenDataFolder);
			SaveDataPathCommand = new DelegateCommand(SaveDataPath);
			ResetDataPathCommand = new DelegateCommand(ResetDataPath);

			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			DataPath = _dataService.DataPath;
		}

		#endregion

		#region Methods

		private void BrowseDataPath()
		{
			var dialog = new FolderBrowserDialog
			{
				ShowNewFolderButton = true,
				SelectedPath = DataPath
			};

			dialog.ShowDialog();
			DataPath = dialog.SelectedPath;
		}

		private void OpenDataFolder()
		{
			if (!IsDataPathCorrect)
				return;

			Process.Start(DataPath);
		}

		private async void SaveDataPath()
		{
			if (!CanSaveDataPath)
				return;

			await _dialogService.ShowChangeDataStoragePopup(this);
		}

		public async Task ChangeDataStorage(bool copyFromCurrentStorage)
		{
			if (copyFromCurrentStorage)
			{
				_dataService.CopyDataTo(DataPath);
				_dataService.SetDataPath(DataPath);
			}
			else
			{
				_dataService.SetDataPath(DataPath);
				await _dataService.InitAsync();
			}

			OnPropertyChanged(nameof(CanSaveDataPath));
		}

		private void ResetDataPath()
		{
			if (IsDataPathDefault)
				return;

			DataPath = _dataService.DefaultDataPath;
		}

		#endregion

		#region Event Handlers

		private void OnDataPathChanged()
		{
			OnPropertyChanged(nameof(IsDataPathCorrect));
			OnPropertyChanged(nameof(IsDataPathDefault));
			OnPropertyChanged(nameof(CanSaveDataPath));
		}

		#endregion
	}
}
