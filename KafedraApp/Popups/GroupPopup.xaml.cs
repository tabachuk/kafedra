using KafedraApp.Commands;
using KafedraApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace KafedraApp.Popups
{
	public partial class GroupPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly TaskCompletionSource<Group> _tcs
			= new TaskCompletionSource<Group>();

		#endregion

		#region Properties

		public Group Group { get; set; }

		public Task<Group> Result => _tcs.Task;

		private string _rateStr;
		public string RateStr
		{
			get => _rateStr;
			set => SetProperty(ref _rateStr, value);
		}

		public bool IsEditMode { get; }

		private string _error;
		public string Error
		{
			get => _error;
			private set => SetProperty(ref _error, value);
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			private set => SetProperty(ref _isBusy, value);
		}

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Commands

		public ICommand SetResultCommand { get; private set; }

		#endregion

		#region Constructors

		public GroupPopup(Group group = null)
		{
			IsEditMode = group != null;
			Group = group ?? new Group { SubgroupsCount = 1, Course = 1 };
			SetResultCommand = new DelegateCommand<Group>(SetResult);

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

		private void SetResult(Group group)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			string error = null;

			var anim = Resources["PopAnimation"] as Storyboard;

			anim.Completed += (o, e) =>
			{
				_tcs.SetResult(group);
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
