using KafedraApp.Commands;
using KafedraApp.Dtos;
using KafedraApp.Models;
using KafedraApp.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Container = KafedraApp.Helpers.Container;

namespace KafedraApp.Popups
{
	public partial class GroupPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly IGroupValidator _groupValidator;

		private readonly TaskCompletionSource<Group> _tcs
			= new TaskCompletionSource<Group>();

		#endregion

		#region Properties

		public GroupDto GroupDto { get; set; }

		public Task<Group> Result => _tcs.Task;

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

		public ICommand SetResultCommand { get; }

		#endregion

		#region Constructors

		public GroupPopup(Group group = null)
		{
			_groupValidator = Container.Resolve<IGroupValidator>();

			IsEditMode = group != null;
			GroupDto = new GroupDto(group);

			if (group == null)
			{
				GroupDto.Course = 1;
				GroupDto.SubgroupsCount = 1;
			}

			SetResultCommand = new DelegateCommand<GroupDto>(SetResult);

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

		private void SetResult(GroupDto groupDto)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			Group group = null;

			if (groupDto != null)
			{
				group = _groupValidator.Validate(groupDto, out string error);

				if (group == null)
				{
					Error = error;
					IsBusy = false;
					return;
				}
			}

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
