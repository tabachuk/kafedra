using KafedraApp.Commands;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
			set => SetProperty(ref _subjects, value);
		}

		#endregion

		#region Constructors

		public SubjectsViewModel()
		{
			_dialogService = Container.Resolve<IDialogService>();

			ImportSubjectsCommand = new DelegateCommand(ImportSubjects);
		}

		#endregion

		#region Commands

		public ICommand ImportSubjectsCommand { get; set; }

		#endregion

		#region Methods

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
	}
}
