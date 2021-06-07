using KafedraApp.Commands;
using KafedraApp.Dtos;
using KafedraApp.Models;
using KafedraApp.Services;
using KafedraApp.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Container = KafedraApp.Helpers.Container;

namespace KafedraApp.Popups
{
	public partial class LoadItemPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly IDataService _dataService;
		private readonly ILoadItemValidator _loadItemValidator;

		private readonly TaskCompletionSource<LoadItem> _tcs
			= new TaskCompletionSource<LoadItem>();

		#endregion

		#region Properties

		public LoadItemDto LoadItemDto { get; set; }

		public Task<LoadItem> Result => _tcs.Task;

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

		public List<string> Teachers { get; set; }

		public List<Group> Groups { get; set; }

		private List<string> _subgroups;
		public List<string> Subgroups
		{
			get => _subgroups;
			set => SetProperty(ref _subgroups, value);
		}

		public List<string> Subjects { get; set; }

		public bool IsSubjectsComboBoxVisible => _loadItemValidator.RequiresSubject(LoadItemDto.Type);

		public bool IsSubgroupsComboBoxVisible => LoadItemDto.Group?.SubgroupsCount > 1;

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Commands

		public ICommand SetResultCommand { get; }

		#endregion

		#region Constructors

		public LoadItemPopup(LoadItem loadItem = null)
		{
			_dataService = Container.Resolve<IDataService>();
			_loadItemValidator = Container.Resolve<ILoadItemValidator>();

			IsEditMode = loadItem != null;
			LoadItemDto = new LoadItemDto(loadItem);
			LoadItemDto.PropertyChanged += OnLoadItemDtoPropertyChanged;
			SetResultCommand = new DelegateCommand<LoadItemDto>(SetResult);

			InitTeachers();
			InitGroups();
			InitSubjects();
			InitializeComponent();
		}

		#endregion

		#region Methods

		private void InitTeachers()
		{
			Teachers = _dataService.Teachers.Select(x => x.LastNameAndInitials).ToList();
			Teachers.Insert(0, LoadItemDto.NoTeacherText);
		}

		private void InitGroups()
		{
			Groups = _dataService.Groups.ToList();

			if (Groups.Any())
			{
				LoadItemDto.Group = Groups.First();
			}
		}

		private void InitSubjects()
		{
			Subjects = _dataService.SubjectNames;
		}

		public override void EndInit()
		{
			base.EndInit();

			var anim = Resources["PushAnimation"] as Storyboard;
			BeginStoryboard(anim);
		}

		private void SetResult(LoadItemDto loadItemDto)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			LoadItem loadItem = null;

			if (loadItemDto != null)
			{
				loadItem = _loadItemValidator.Validate(loadItemDto, out string error);

				if (loadItem == null)
				{
					Error = error;
					IsBusy = false;
					return;
				}
			}

			var anim = Resources["PopAnimation"] as Storyboard;

			anim.Completed += (o, e) =>
			{
				_tcs.SetResult(loadItem);
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

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Event Handlers

		private void OnLoadItemDtoPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(LoadItemDto.Group))
			{
				var subgroups = new List<string> { LoadItemDto.NoSubgroupText };

				if (LoadItemDto.Group.SubgroupsCount > 1)
				{
					subgroups.AddRange(Enumerable.Range(1, LoadItemDto.Group.SubgroupsCount)
						.Select(x => x.ToString()));
				}

				Subgroups = subgroups;
				LoadItemDto.Subgroup = Subgroups.First();

				RaisePropertyChanged(nameof(IsSubgroupsComboBoxVisible));
			}
			else if (e.PropertyName == nameof(LoadItemDto.Type))
			{
				RaisePropertyChanged(nameof(IsSubjectsComboBoxVisible));
			}
		}

		#endregion
	}
}
