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
	public partial class SubjectPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly IDataService _dataService;
		private readonly ISubjectValidator _subjectValidator;

		private readonly TaskCompletionSource<Subject> _tcs
			= new TaskCompletionSource<Subject>();

		#endregion

		#region Properties

		public SubjectDto SubjectDto { get; set; }

		public Task<Subject> Result => _tcs.Task;

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

		public List<string> Specialties { get; set; }

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Commands

		public ICommand SetResultCommand { get; }

		#endregion

		#region Constructors

		public SubjectPopup(Subject subject = null)
		{
			_dataService = Container.Resolve<IDataService>();
			_subjectValidator = Container.Resolve<ISubjectValidator>();

			IsEditMode = subject != null;
			SubjectDto = new SubjectDto(subject);

			if (subject == null)
			{
				SubjectDto.Course = 1;
				SubjectDto.Semester = 1;
			}

			SetResultCommand = new DelegateCommand<SubjectDto>(SetResult);

			InitSpecialties();
			InitializeComponent();
		}

		#endregion

		#region Methods

		private void InitSpecialties()
		{
			Specialties = _dataService.Groups?
				.Select(x => x.Specialty)?
				.Distinct()?.ToList();
		}

		public override void EndInit()
		{
			base.EndInit();

			var anim = Resources["PushAnimation"] as Storyboard;
			BeginStoryboard(anim);
		}

		private void SetResult(SubjectDto subjectDto)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			Subject subject = null;

			if (subjectDto != null)
			{
				subject = _subjectValidator.Validate(subjectDto, out string error);

				if (subject == null)
				{
					Error = error;
					IsBusy = false;
					return;
				}
			}

			var anim = Resources["PopAnimation"] as Storyboard;

			anim.Completed += (o, e) =>
			{
				_tcs.SetResult(subject);
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
