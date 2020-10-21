using KafedraApp.Commands;
using KafedraApp.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class SubjectsViewModel : BindableBase
	{
		#region Constructors

		public SubjectsViewModel()
		{
			ImportSubjectsCommand = new DelegateCommand(async () => await ImportSubjects());
		}

		#endregion

		#region Commands

		public ICommand ImportSubjectsCommand { get; set; }

		#endregion

		#region Methods

		private async Task ImportSubjects()
		{
			var dialog = new OpenFileDialog
			{
				Filter = "Excel files|*.xls;*.xlsx;*.xlsm"
			};

			if (dialog.ShowDialog() != true)
				return;

			await Task.Run(() => ExcelHelper.GetSubjects(dialog.FileName));
		}

		#endregion
	}
}
