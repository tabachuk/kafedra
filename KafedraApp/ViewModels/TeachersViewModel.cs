using KafedraApp.Commands;
using KafedraApp.Extensions;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class TeachersViewModel : BindableBase
	{
		#region Fields

		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		#endregion

		#region Properties

		public ObservableCollection<Teacher> Teachers => _dataService.Teachers;

		public bool IsTeachersEmpty => Teachers?.Any() != true;

		#endregion

		#region Commands

		public ICommand AddTeacherCommand { get; }
		public ICommand EditTeacherCommand { get; }
		public ICommand DeleteTeacherCommand { get; }
		public ICommand ClearTeachersCommand { get; }
		public ICommand EditSubjectsCommand { get; }

		#endregion

		#region Constructors

		public TeachersViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			AddTeacherCommand = new DelegateCommand(AddTeacher);
			EditTeacherCommand = new DelegateCommand<Teacher>(EditTeacher);
			DeleteTeacherCommand = new DelegateCommand<Teacher>(DeleteTeacher);
			ClearTeachersCommand = new DelegateCommand(ClearTeachers);
			EditSubjectsCommand = new DelegateCommand<Teacher>(EditSubjects);

			Teachers.CollectionChanged += OnTeachersChanged;
		}

		#endregion

		#region Methods

		private async void AddTeacher()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var teacher = await _dialogService.ShowTeacherForm();

			if (teacher != null)
				Teachers?.Add(teacher);

			IsBusy = false;
		}

		private async void EditTeacher(Teacher teacher)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			teacher = await _dialogService.ShowTeacherForm(teacher.Clone() as Teacher);

			if (teacher != null)
			{
				var id = Teachers.IndexOf(Teachers.GetById(teacher.Id));
				Teachers[id] = teacher;
			}

			IsBusy = false;
		}

		private async void DeleteTeacher(Teacher teacher)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var res = await _dialogService.ShowQuestion(
				$"Ви дійсно бажаєте видалити { teacher.FullName }?");

			if (res)
				Teachers.Remove(teacher);

			IsBusy = false;
		}

		private async void ClearTeachers()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var res = await _dialogService.ShowQuestion(
				"Ви дійсно бажаєте видалити всіх викладачів?");

			if (res)
				Teachers.Clear();

			IsBusy = false;
		}

		private async void EditSubjects(Teacher teacher)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			if (_dataService.SubjectNames?.Any() != true)
			{
				await _dialogService.ShowError("Спочатку додайте предмети в базу даних");
				return;
			}

			var subjects = await _dialogService.ShowSubjectPickerPopup(
				teacher.FullName,
				teacher.SubjectsSpecializesIn?.ToList());

			if (subjects != null)
			{
				teacher.SubjectsSpecializesIn =
					new ObservableCollection<string>(subjects);

				var id = Teachers.IndexOf(Teachers.GetById(teacher.Id));
				Teachers[id] = teacher;
			}

			IsBusy = false;
		}

		#endregion

		#region Event Handlers

		private void OnTeachersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsTeachersEmpty));
		}

		#endregion
	}
}
