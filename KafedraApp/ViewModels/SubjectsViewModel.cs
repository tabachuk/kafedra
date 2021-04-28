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

		public ObservableCollection<Subject> Subjects { get; }

		public List<Subject> OrderedFilteredSubjects
		{
			get
			{
				if (Subjects?.Any() != true)
				{
					return Subjects?.ToList();
				}

				var filteredSubjects = FilterSubjects(Subjects, SearchText);
				var orderedFilteredSubjects = OrderSubjects(filteredSubjects, SelectedSortingField);

				return orderedFilteredSubjects.ToList();
			}
		}

		private ObservableCollection<Subject> _subjectsToShow;
		public ObservableCollection<Subject> SubjectsToShow
		{
			get => _subjectsToShow;
			set
			{
				if (_subjectsToShow != null)
					_subjectsToShow.CollectionChanged -= OnSubjectsToShowChanged;

				SetProperty(ref _subjectsToShow, value);
				OnPropertyChanged(nameof(IsSubjectNotFound));

				if (_subjectsToShow != null)
					_subjectsToShow.CollectionChanged -= OnSubjectsToShowChanged;
			}
		}

		public bool IsSubjectsEmpty => Subjects?.Any() != true;

		public bool IsSubjectNotFound => !IsSubjectsEmpty && _subjectsToShow?.Any() != true;

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set => SetProperty(ref _searchText, value, OnSearchTextChanged);
		}

		public List<string> SortingFields => new List<string>
		{
			"Предмет",
			"Спеціальність",
			"Курс",
			"Семестр",
			"Кредити",
			"Всього годин",
			"Всього годин (аудиторних)",
			"Лекції",
			"Практичні",
			"Лабораторні",
			"Підсумковий контроль"
		};

		private string _selectedSortingField;
		public string SelectedSortingField
		{
			get => _selectedSortingField;
			set => SetProperty(ref _selectedSortingField, value, OnSelectedSortingFieldChanged);
		}

		#endregion

		#region Constructors

		public SubjectsViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			Subjects = _dataService.Subjects;

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

		public ICommand ImportSubjectsCommand { get; }
		public ICommand AddSubjectCommand { get; }
		public ICommand EditSubjectCommand { get; }
		public ICommand DeleteSubjectCommand { get; }
		public ICommand ClearSubjectsCommand { get; }

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
			if (OrderedFilteredSubjects?.Any() == true)
			{
				SubjectsToShow = new ObservableCollection<Subject>(
					OrderedFilteredSubjects.Take(Math.Min(MaxSubjectsToShow, OrderedFilteredSubjects.Count)));
			}
			else
			{
				SubjectsToShow = new ObservableCollection<Subject>();
			}
		}

		public void AddSubjectToShow()
		{
			if (SubjectsToShow.Count >= OrderedFilteredSubjects.Count)
				return;

			SubjectsToShow.Add(OrderedFilteredSubjects[SubjectsToShow.Count]);
		}

		private IEnumerable<Subject> FilterSubjects(IEnumerable<Subject> subjects, string searchText)
		{
			if (string.IsNullOrWhiteSpace(searchText))
			{
				return subjects;
			}

			return subjects.Where(x => x.Name.ToLower().Contains(searchText.ToLower()));
		}

		private IEnumerable<Subject> OrderSubjects(IEnumerable<Subject> subjects, string byField)
		{
			switch (byField)
			{
				case "Предмет":
					return OrderBy(subjects, x => x.Name);
				case "Спеціальність":
					return OrderBy(subjects, x => x.Specialty);
				case "Курс":
					return OrderBy(subjects, x => x.Course);
				case "Семестр":
					return OrderBy(subjects, x => x.Semester);
				case "Кредити":
					return OrderBy(subjects, x => x.Credits);
				case "Всього годин":
					return OrderBy(subjects, x => x.TotalHours);
				case "Всього годин (аудиторних)":
					return OrderBy(subjects, x => x.TotalClassroomHours);
				case "Лекції":
					return OrderBy(subjects, x => x.LectureHours);
				case "Практичні":
					return OrderBy(subjects, x => x.PracticalWorkHours);
				case "Лабораторні":
					return OrderBy(subjects, x => x.LaboratoryWorkHours);
				case "Підсумковий контроль":
					return OrderBy(subjects, x => x.FinalControlFormType);
			}

			return subjects;
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
					var subject = e.NewItems[0] as Subject;
					_dataService.Subjects.Add(subject);

					if (_isImporting)
						break;

					var index = OrderedFilteredSubjects.IndexOf(subject);

					if (index >= 0 && index <= SubjectsToShow.Count)
					{
						SubjectsToShow.Insert(index, subject);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					subject = e.OldItems[0] as Subject;
					_dataService.Subjects.Remove(subject);
					SubjectsToShow.Remove(subject);
					AddSubjectToShow();
					break;
				case NotifyCollectionChangedAction.Replace:
					var newSubject = e.NewItems[0] as Subject;

					index = OrderedFilteredSubjects.IndexOf(newSubject);

					if (index >= 0 && index <= SubjectsToShow.Count)
					{
						SubjectsToShow.RemoveAt(index);
						SubjectsToShow.Insert(index, newSubject);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					_dataService.Subjects.Clear();
					SubjectsToShow.Clear();
					break;
			}
		}

		private void OnSubjectsToShowChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsSubjectNotFound));
		}

		private void OnSearchTextChanged()
		{
			InitSubjectsToShow();
		}

		private void OnSelectedSortingFieldChanged()
		{
			InitSubjectsToShow();
		}

		#endregion
	}
}
