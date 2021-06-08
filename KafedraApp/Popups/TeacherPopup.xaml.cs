using KafedraApp.Commands;
using KafedraApp.Dtos;
using KafedraApp.Models;
using KafedraApp.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Container = KafedraApp.Helpers.Container;

namespace KafedraApp.Popups
{
	public partial class TeacherPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly ITeacherValidator _teacherValidator;

		private readonly TaskCompletionSource<Teacher> _tcs
			= new TaskCompletionSource<Teacher>();

		#endregion

		#region Properties

		public TeacherDto TeacherDto { get; set; }

		public Task<Teacher> Result => _tcs.Task;

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

		public TeacherPopup(Teacher teacher = null)
		{
			_teacherValidator = Container.Resolve<ITeacherValidator>();

			IsEditMode = teacher != null;
			TeacherDto = new TeacherDto(teacher);
			SetResultCommand = new DelegateCommand<TeacherDto>(SetResult);

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

		private void SetResult(TeacherDto teacherDto)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			Teacher teacher = null;

			if (teacherDto != null)
			{
				teacher = _teacherValidator.Validate(teacherDto, out string error);

				if (teacher == null)
				{
					Error = error;
					IsBusy = false;
					return;
				}
			}

			var anim = Resources["PopAnimation"] as Storyboard;

			anim.Completed += (o, e) =>
			{
				_tcs.SetResult(teacher);
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
