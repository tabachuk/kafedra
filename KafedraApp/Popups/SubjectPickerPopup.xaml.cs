using KafedraApp.Commands;
using KafedraApp.Extensions;
using KafedraApp.Helpers;
using KafedraApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace KafedraApp.Popups
{
	public partial class SubjectPickerPopup
	{
		#region Fields

		private readonly IDataService _dataService;

		private readonly TaskCompletionSource<List<string>> _tcs
			= new TaskCompletionSource<List<string>>();

		#endregion

		#region Properties

		public string TeacherName { get; }

		public ObservableCollection<string> NotTeacherSubjects { get; set; }

		public ObservableCollection<string> TeacherSubjects { get; set; }

		public Task<List<string>> Result => _tcs.Task;

		public bool IsBusy { get; private set; }

		#endregion

		#region Commands

		public ICommand SetResultCommand { get; private set; }
		public ICommand MoveSubjectToLeftColumnCommand { get; private set; }
		public ICommand MoveSubjectToRightColumnCommand { get; private set; }

		#endregion

		#region Constructors

		public SubjectPickerPopup(string teacherName, List<string> subjects)
		{
			_dataService = Container.Resolve<IDataService>();

			TeacherName = teacherName;
			InitSubjects(subjects);

			SetResultCommand = new DelegateCommand<bool>(SetResult);
			MoveSubjectToLeftColumnCommand = new DelegateCommand<string>(MoveSubjectToLeftColumn);
			MoveSubjectToRightColumnCommand = new DelegateCommand<string>(MoveSubjectToRightColumn);

			InitializeComponent();
		}

		#endregion

		#region Methods

		private void InitSubjects(List<string> subjects)
		{
			if (subjects?.Any() == true)
			{
				subjects.Sort();
				TeacherSubjects = new ObservableCollection<string>(subjects);

				if (_dataService.SubjectNames?.Any() == true)
				{
					var notTeacherSubjects = _dataService.SubjectNames
						.Except(TeacherSubjects);

					NotTeacherSubjects =
						new ObservableCollection<string>(notTeacherSubjects);
				}
				else
				{
					NotTeacherSubjects = new ObservableCollection<string>();
				}
			}
			else
			{
				TeacherSubjects = new ObservableCollection<string>();

				if (_dataService.SubjectNames?.Any() == true)
				{
					NotTeacherSubjects =
						new ObservableCollection<string>(_dataService.SubjectNames);
				}
				else
				{
					NotTeacherSubjects = new ObservableCollection<string>();
				}
			}
		}

		public override void EndInit()
		{
			base.EndInit();

			var anim = Resources["PushAnimation"] as Storyboard;
			BeginStoryboard(anim);
		}

		private void SetResult(bool result)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var anim = Resources["PopAnimation"] as Storyboard;

			anim.Completed += (o, e) =>
			{
				_tcs.SetResult(result ? TeacherSubjects.ToList() : null);
				IsBusy = false;
			};

			BeginStoryboard(anim);
		}

		private void MoveSubjectToLeftColumn(string subject)
		{
			TeacherSubjects.Remove(subject);
			var orderIndex = NotTeacherSubjects.GetOrderIndex(subject);
			NotTeacherSubjects.Insert(orderIndex, subject);
		}

		private void MoveSubjectToRightColumn(string subject)
		{
			NotTeacherSubjects.Remove(subject);
			var orderIndex = TeacherSubjects.GetOrderIndex(subject);
			TeacherSubjects.Insert(orderIndex, subject);
		}

		#endregion
	}
}
