using KafedraApp.Commands;
using KafedraApp.Extensions;
using KafedraApp.Helpers;
using KafedraApp.Services;
using System;
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

		public ObservableCollection<SubjectViewDto> NotTeacherSubjects { get; set; }

		public ObservableCollection<SubjectViewDto> TeacherSubjects { get; set; }

		public Task<List<string>> Result => _tcs.Task;

		public bool IsBusy { get; private set; }

		#endregion

		#region Commands

		public ICommand SetResultCommand { get; }
		public ICommand MoveSubjectToLeftColumnCommand { get; }
		public ICommand MoveSubjectToRightColumnCommand { get; }

		#endregion

		#region Constructors

		public SubjectPickerPopup(string teacherName, List<string> subjects)
		{
			_dataService = Container.Resolve<IDataService>();

			TeacherName = teacherName;
			InitSubjects(subjects);

			SetResultCommand = new DelegateCommand<bool>(SetResult);
			MoveSubjectToLeftColumnCommand = new DelegateCommand<SubjectViewDto>(MoveSubjectToLeftColumn);
			MoveSubjectToRightColumnCommand = new DelegateCommand<SubjectViewDto>(MoveSubjectToRightColumn);

			InitializeComponent();
		}

		#endregion

		#region Methods

		private void InitSubjects(List<string> subjects)
		{
			if (subjects?.Any() == true)
			{
				subjects.Sort();
				TeacherSubjects = new ObservableCollection<SubjectViewDto>(
					subjects.Select(x => new SubjectViewDto(x, true)));

				if (_dataService.SubjectNames?.Any() == true)
				{
					var notTeacherSubjects = _dataService.SubjectNames
						.Except(subjects);

					NotTeacherSubjects = new ObservableCollection<SubjectViewDto>(
						notTeacherSubjects.Select(x => new SubjectViewDto(x)));
				}
				else
				{
					NotTeacherSubjects = new ObservableCollection<SubjectViewDto>();
				}
			}
			else
			{
				TeacherSubjects = new ObservableCollection<SubjectViewDto>();

				if (_dataService.SubjectNames?.Any() == true)
				{
					NotTeacherSubjects = new ObservableCollection<SubjectViewDto>(
						_dataService.SubjectNames.Select(x => new SubjectViewDto(x)));
				}
				else
				{
					NotTeacherSubjects = new ObservableCollection<SubjectViewDto>();
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
				_tcs.SetResult(result ? TeacherSubjects.Select(x => x.Name).ToList() : null);
				IsBusy = false;
			};

			BeginStoryboard(anim);
		}

		private void MoveSubjectToLeftColumn(SubjectViewDto subject)
		{
			subject.IsAssignedToTeacher = false;
			TeacherSubjects.Remove(subject);
			var orderIndex = NotTeacherSubjects.GetOrderIndex(subject);
			NotTeacherSubjects.Insert(orderIndex, subject);
		}

		private void MoveSubjectToRightColumn(SubjectViewDto subject)
		{
			subject.IsAssignedToTeacher = true;
			NotTeacherSubjects.Remove(subject);
			var orderIndex = TeacherSubjects.GetOrderIndex(subject);
			TeacherSubjects.Insert(orderIndex, subject);
		}

		#endregion
	}

	public class SubjectViewDto : IComparable
	{
		public string Name { get; set; }

		public bool IsAssignedToTeacher { get; set; }

		public SubjectViewDto(string name, bool isAssignedToTeacher = false)
		{
			Name = name;
			IsAssignedToTeacher = isAssignedToTeacher;
		}

		public int CompareTo(object obj)
		{
			var subject = obj as SubjectViewDto;
			return Name.CompareTo(subject.Name);
		}
	}
}
