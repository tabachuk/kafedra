using KafedraApp.Commands;
using KafedraApp.Extensions;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class SubjectsViewModel : BindableBase
	{
		#region Constants

		private const int MaxSubjectsToShow = 20;

		#endregion

		#region Fields

		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private bool _isImporting;

		#endregion

		#region Properties

		public ObservableCollection<Subject> Subjects => _dataService.Subjects;

		public List<Subject> FilteredSubjects
		{
			get
			{
				if (Subjects?.Any() != true)
				{
					return Subjects?.ToList();
				}

				IEnumerable<Subject> filteredSubjects = Subjects;

				if (!string.IsNullOrWhiteSpace(SearchText))
				{
					filteredSubjects = Subjects
						.Where(x => x.Name.ToLower().Contains(SearchText.ToLower()))
						.ToList();
				}

				switch (SelectedSortingField)
				{
					case "Предмет":
						filteredSubjects = OrderBy(filteredSubjects, x => x.Name);
						break;
					case "Спеціальність":
						filteredSubjects = OrderBy(filteredSubjects, x => x.Specialty);
						break;
					case "Курс":
						filteredSubjects = OrderBy(filteredSubjects, x => x.Course);
						break;
					case "Семестр":
						filteredSubjects = OrderBy(filteredSubjects, x => x.Semester);
						break;
					case "Кредити":
						filteredSubjects = OrderBy(filteredSubjects, x => x.Credits);
						break;
					case "Всього":
						filteredSubjects = OrderBy(filteredSubjects, x => x.TotalHours);
						break;
					case "Всього (аудиторних)":
						filteredSubjects = OrderBy(filteredSubjects, x => x.TotalClassroomHours);
						break;
					case "Лекції":
						filteredSubjects = OrderBy(filteredSubjects, x => x.LectureHours);
						break;
					case "Практичні":
						filteredSubjects = OrderBy(filteredSubjects, x => x.PracticalWorkHours);
						break;
					case "Лабораторні":
						filteredSubjects = OrderBy(filteredSubjects, x => x.LaboratoryWorkHours);
						break;
					case "Екзамен":
						filteredSubjects = OrderBy(filteredSubjects, x => x.ExamHours);
						break;
					case "Залік":
						filteredSubjects = OrderBy(filteredSubjects, x => x.TestHours);
						break;
				}

				return filteredSubjects.ToList();
			}
		}

		private ObservableCollection<Subject> _subjectsToShow;
		public ObservableCollection<Subject> SubjectsToShow
		{
			get => _subjectsToShow;
			set => SetProperty(ref _subjectsToShow, value);
		}

		public bool IsSubjectsEmpty => FilteredSubjects?.Any() != true;

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				SetProperty(ref _searchText, value);
				OnSearchTextChanged();
			}
		}

		public List<string> SortingFields => new List<string>
		{
			"Предмет",
			"Спеціальність",
			"Курс",
			"Семестр",
			"Кредити",
			"Всього",
			"Всього (аудиторних)",
			"Лекції",
			"Практичні",
			"Лабораторні",
			"Екзамен",
			"Залік"
		};

		private string _selectedSortingField;
		public string SelectedSortingField
		{
			get => _selectedSortingField;
			set
			{
				SetProperty(ref _selectedSortingField, value);
				OnSelectedSortingFieldChanged();
			}
		}

		#endregion

		#region Constructors

		public SubjectsViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			SelectedSortingField = SortingFields[0];

			InitSubjectsToShow();

			AddSubjectCommand = new DelegateCommand(AddSubject);
			EditSubjectCommand = new DelegateCommand<Subject>(EditSubject);
			DeleteSubjectCommand = new DelegateCommand<Subject>(DeleteSubject);
			ImportSubjectsCommand = new DelegateCommand(ImportSubjects);
			ClearSubjectsCommand = new DelegateCommand(ClearSubjects);

			Subjects.CollectionChanged += OnSubjectsChanged;
		}

		#endregion

		#region Commands

		public ICommand ImportSubjectsCommand { get; set; }
		public ICommand AddSubjectCommand { get; set; }
		public ICommand EditSubjectCommand { get; set; }
		public ICommand DeleteSubjectCommand { get; set; }
		public ICommand ClearSubjectsCommand { get; set; }

		#endregion

		#region Methods

		private async void AddSubject()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var subject = await _dialogService.ShowSubjectForm();

			if (subject != null)
			{
				Subjects?.Add(subject);
				await _dataService.SaveSubjects();
			}

			IsBusy = false;
		}

		private async void EditSubject(Subject subject)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			subject = await _dialogService.ShowSubjectForm(subject.Clone() as Subject);

			if (subject != null)
			{
				var id = Subjects.IndexOf(Subjects.GetById(subject.Id));
				Subjects[id] = subject;
				await _dataService.SaveSubjects();
			}

			IsBusy = false;
		}

		private async void DeleteSubject(Subject subject)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var res = await _dialogService.ShowQuestion(
				$"Ви дійсно бажаєте видалити { subject.Name }?");

			if (res)
			{
				Subjects.Remove(subject);
				await _dataService.SaveSubjects();
			}

			IsBusy = false;
		}

		private void ImportSubjects()
		{
			var dialog = new OpenFileDialog
			{
				Filter = "Excel files|*.xls;*.xlsx;*.xlsm",
				Multiselect = true
			};

			if (dialog.ShowDialog() != true)
				return;

			_dialogService.ShowSubjectImportPopup();

			Task.Run(async () =>
			{
				_isImporting = true;
				var subjects = ExcelHelper.GetSubjects(dialog.FileNames);

				foreach (var subject in subjects)
				{
					Application.Current.Dispatcher.Invoke(() =>
						Subjects.Add(subject));
				}

				await _dataService.SaveSubjects();
				_isImporting = false;
			});
		}

		private async void ClearSubjects()
		{
			var res = await _dialogService.ShowQuestion(
				"Ви дійсно бажаєте видалити всі предмети?");

			if (res)
			{
				Subjects.Clear();
				await _dataService.SaveSubjects();
			}
		}

		private void InitSubjectsToShow()
		{
			if (FilteredSubjects?.Any() == true)
			{
				SubjectsToShow = new ObservableCollection<Subject>(
					FilteredSubjects.Take(Math.Min(MaxSubjectsToShow, FilteredSubjects.Count)));
			}
			else
			{
				SubjectsToShow = new ObservableCollection<Subject>();
			}
		}

		public void AddSubjectToShow()
		{
			if (SubjectsToShow.Count >= FilteredSubjects.Count)
				return;

			SubjectsToShow.Add(FilteredSubjects[SubjectsToShow.Count]);
		}

		private IEnumerable<Subject> OrderBy<TKey>(
			IEnumerable<Subject> subjects,
			Func<Subject, TKey> keySelector,
			bool byDescending = false)
		{
			if (byDescending)
			{
				return subjects.OrderByDescending(keySelector);
			}

			return subjects.OrderBy(keySelector);
		}

		#endregion

		#region Event Handlers

		private void OnSubjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsSubjectsEmpty));

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (_isImporting && SubjectsToShow.Count >= MaxSubjectsToShow)
						break;

					var subject = e.NewItems[0] as Subject;
					var index = FilteredSubjects.IndexOf(subject);

					if (index >= 0 && index <= SubjectsToShow.Count)
					{
						SubjectsToShow.Insert(index, subject);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					subject = e.OldItems[0] as Subject;
					SubjectsToShow.Remove(subject);
					AddSubjectToShow();
					break;
				case NotifyCollectionChangedAction.Replace:
					var newSubject = e.NewItems[0] as Subject;

					index = FilteredSubjects.IndexOf(newSubject);

					if (index >= 0 && index <= SubjectsToShow.Count)
					{
						SubjectsToShow.RemoveAt(index);
						SubjectsToShow.Insert(index, newSubject);
					}
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Reset:
					SubjectsToShow.Clear();
					break;
			}
		}

		private void OnSearchTextChanged()
		{
			InitSubjectsToShow();
			OnPropertyChanged(nameof(IsSubjectsEmpty));
		}

		private void OnSelectedSortingFieldChanged()
		{
			InitSubjectsToShow();
		}

		#endregion
	}
}
