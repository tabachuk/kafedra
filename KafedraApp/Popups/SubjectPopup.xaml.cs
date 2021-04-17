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
	public partial class SubjectPopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly TaskCompletionSource<Subject> _tcs
			= new TaskCompletionSource<Subject>();

		#endregion

		#region Properties

		public Subject Subject { get; set; }

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
			IsEditMode = subject != null;
			Subject = subject ?? new Subject();
			SetResultCommand = new DelegateCommand<Subject>(SetResult);

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

		private void SetResult(Subject subject)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			string error = null;

			//if (subject != null)
			//{
			//	if (string.IsNullOrWhiteSpace(RateStr))
			//	{
			//		Error = "Вкажіть ставку";
			//		IsBusy = false;
			//		return;
			//	}

			//	if (!float.TryParse(
			//			RateStr,
			//			NumberStyles.Any,
			//			CultureInfo.InvariantCulture,
			//			out float rate))
			//	{
			//		Error = "Невірний формат ставки";
			//		IsBusy = false;
			//		return;
			//	}

			//	subject.Rate = rate;

			//	if (!subject.IsValid(out error))
			//	{
			//		Error = error;
			//		IsBusy = false;
			//		return;
			//	}
			//}

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
