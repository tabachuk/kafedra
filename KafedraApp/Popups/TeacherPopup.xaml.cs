using KafedraApp.Commands;
using KafedraApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace KafedraApp.Popups
{
	public partial class TeacherPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly TaskCompletionSource<Teacher> _tcs
			= new TaskCompletionSource<Teacher>();

		#endregion

		#region Properties

		public Teacher Teacher { get; set; }

		public Task<Teacher> Result => _tcs.Task;

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

		public TeacherPopup(Teacher teacher = null)
		{
			IsEditMode = teacher != null;
			Teacher = teacher ?? new Teacher { Rate = 1 };
			RateStr = Teacher.Rate.ToString(CultureInfo.InvariantCulture);
			SetResultCommand = new DelegateCommand<Teacher>(SetResult);

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

		private void SetResult(Teacher teacher)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			string error = null;

			if (teacher != null)
			{
				if (string.IsNullOrWhiteSpace(RateStr))
				{
					Error = "Вкажіть ставку";
					IsBusy = false;
					return;
				}

				if (!float.TryParse(
						RateStr,
						NumberStyles.Any,
						CultureInfo.InvariantCulture,
						out float rate))
				{
					Error = "Невірний формат ставки";
					IsBusy = false;
					return;
				}

				teacher.Rate = rate;

				if (!teacher.IsValid(out error))
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
