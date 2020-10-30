using KafedraApp.Commands;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using Microsoft.Win32;
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

		private readonly IDialogService _dialogService;

		#endregion

		#region Properties

		private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
		public ObservableCollection<Subject> Subjects
		{
			get => _subjects;
			set
			{
				SetProperty(ref _subjects, value);
				OnPropertyChanged(nameof(IsSubjectsEmpty));
			}
		}

		public bool IsSubjectsEmpty => Subjects?.Any() != true;

		#endregion

		#region Constructors

		public SubjectsViewModel()
		{
			_dialogService = Container.Resolve<IDialogService>();

			AddSubjectCommand = new DelegateCommand(AddSubject);
			ImportSubjectsCommand = new DelegateCommand(ImportSubjects);

			Subjects.CollectionChanged += SubjectsChanged;
		}

		#endregion

		#region Commands

		public ICommand ImportSubjectsCommand { get; set; }
		public ICommand AddSubjectCommand { get; set; }

		#endregion

		#region Methods

		private async void AddSubject()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var subject = await _dialogService.ShowSubjectForm();

			if (subject != null)
				Subjects?.Add(subject);

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
				var subjects = ExcelHelper.GetSubjects(dialog.FileNames);

				foreach (var subject in subjects)
				{
					await Task.Delay(100);

					Application.Current.Dispatcher.Invoke(() =>
						Subjects.Add(subject));
				}
			});
		}

		#endregion

		#region Event Handlers

		private void SubjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsSubjectsEmpty));
		}

		#endregion
	}
}
