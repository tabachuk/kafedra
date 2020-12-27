using KafedraApp.Commands;
using KafedraApp.Extensions;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using Microsoft.Win32;
using System;
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
		#region Fields

		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private bool _isImporting;

		#endregion

		#region Properties

		public ObservableCollection<Subject> Subjects => _dataService.Subjects;

		public ObservableCollection<Subject> SubjectsToShow { get; set; }

		public bool IsSubjectsEmpty => Subjects?.Any() != true;

		#endregion

		#region Constructors

		public SubjectsViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

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
			if (Subjects?.Any() == true)
			{
				SubjectsToShow = new ObservableCollection<Subject>(
					Subjects.Take(Math.Min(10, Subjects.Count)));
			}
			else
			{
				SubjectsToShow = new ObservableCollection<Subject>();
			}
		}

		public void AddSubjectToShow()
		{
			if (SubjectsToShow.Count >= Subjects.Count)
				return;

			SubjectsToShow.Add(Subjects[SubjectsToShow.Count]);
		}

		#endregion

		#region Event Handlers

		private void OnSubjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsSubjectsEmpty));

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (_isImporting && SubjectsToShow.Count >= 10)
						break;

					var subject = e.NewItems[0] as Subject;
					var index = Subjects.IndexOf(subject);

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

					index = Subjects.IndexOf(newSubject);

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

		#endregion
	}
}
