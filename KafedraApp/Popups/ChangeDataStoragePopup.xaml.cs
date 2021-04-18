using KafedraApp.Commands;
using KafedraApp.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace KafedraApp.Popups
{
	public enum ChangeDataStorageState
	{
		WaitingForUserResponse,
		Processing,
		Success
	};

	public partial class ChangeDataStoragePopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly TaskCompletionSource<bool> _tcs
			= new TaskCompletionSource<bool>();

		#endregion

		#region Properties

		private SettingsViewModel ViewModel => DataContext as SettingsViewModel;

		private ChangeDataStorageState _changeDataStorageState;
		public ChangeDataStorageState ChangeDataStorageState
		{
			get => _changeDataStorageState;
			private set => SetProperty(ref _changeDataStorageState, value);
		}

		public Task<bool> Result => _tcs.Task;

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			private set => SetProperty(ref _isBusy, value);
		}

		private bool _copyFromCurrentStorage;
		public bool CopyFromCurrentStorage
		{
			get => _copyFromCurrentStorage;
			set => SetProperty(ref _copyFromCurrentStorage, value);
		}

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Commands

		public ICommand SaveCommand { get; }
		public ICommand CloseCommand { get; }

		#endregion

		#region Constructors

		public ChangeDataStoragePopup()
		{
			SaveCommand = new DelegateCommand(Save);
			CloseCommand = new DelegateCommand(() => Close());

			InitializeComponent();
		}

		#endregion

		#region Methods

		public override void EndInit()
		{
			base.EndInit();

			var anim = Resources["PushAnimation"] as Storyboard;
			BeginStoryboard(anim);
		}

		private async void Save()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			ChangeDataStorageState = ChangeDataStorageState.Processing;
			await ViewModel.ChangeDataStorage(CopyFromCurrentStorage);

			ChangeDataStorageState = ChangeDataStorageState.Success;
			await Task.Delay(500);

			Close(true);
		}

		private void Close(bool afterSave = false)
		{
			if (!afterSave)
			{
				if (IsBusy || ChangeDataStorageState != ChangeDataStorageState.WaitingForUserResponse)
					return;

				IsBusy = true;
			}

			var anim = Resources["PopAnimation"] as Storyboard;

			anim.Completed += (o, e) =>
			{
				_tcs.SetResult(true);
				IsBusy = false;
			};

			BeginStoryboard(anim);
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
	}
}
