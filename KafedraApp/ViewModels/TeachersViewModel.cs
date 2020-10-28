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

		public ICommand AddTeacherCommand { get; private set; }
		public ICommand EditTeacherCommand { get; private set; }
		public ICommand DeleteTeacherCommand { get; private set; }

		#endregion

		#region Constructors

		public TeachersViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			AddTeacherCommand = new DelegateCommand(AddTeacher);
			EditTeacherCommand = new DelegateCommand<Teacher>(EditTeacher);
			DeleteTeacherCommand = new DelegateCommand<Teacher>(DeleteTeacher);

			Teachers.CollectionChanged += TeachersChanged;
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
				$"Ви дійсно бажаєте видалити { teacher.LastName } { teacher.FirstName } { teacher.MiddleName }?");

			if (res)
				Teachers.Remove(teacher);

			IsBusy = false;
		}

		#endregion

		#region Event Handlers

		private void TeachersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsTeachersEmpty));
		}

		#endregion
	}
}
