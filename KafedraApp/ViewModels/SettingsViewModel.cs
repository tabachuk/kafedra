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

		#endregion

		#region Properties

		private string _dataPath;
		public string DataPath
		{
			get => _dataPath;
			set => SetProperty(ref _dataPath, value, OnDataPathChanged);
		}

		public bool IsDataPathCorrect => Directory.Exists(DataPath);

		#endregion

		#region Commands

		public ICommand BrowseDataPathCommand { get; }
		public ICommand OpenDataFolderCommand { get; }
		public ICommand SaveDataPathCommand { get; }

		#endregion

		#region Constructors

		public SettingsViewModel()
		{
			BrowseDataPathCommand = new DelegateCommand(BrowseDataPath);
			OpenDataFolderCommand = new DelegateCommand(OpenDataFolder);
			SaveDataPathCommand = new DelegateCommand(async () => await SaveDataPath());

			_dataService = Container.Resolve<IDataService>();

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

		private async Task SaveDataPath()
		{
			if (!IsDataPathCorrect || _dataService.DataPath == DataPath)
				return;

			_dataService.SetDataPath(DataPath);
			await _dataService.InitAsync();
		}

		#endregion

		#region Event Handlers

		private void OnDataPathChanged()
		{
			OnPropertyChanged(nameof(IsDataPathCorrect));
		}

		#endregion
	}
}
