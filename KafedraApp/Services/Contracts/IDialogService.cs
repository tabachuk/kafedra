﻿using KafedraApp.Models;
using KafedraApp.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafedraApp.Services
{
	public interface IDialogService
	{
		Task ShowError(
			string message,
			string caption = "Помилка");
		Task ShowInfo(
			string message,
			string caption = "Інформація");
		Task<bool> ShowQuestion(
			string message,
			string OKButtonText = "ОК",
			string cancelButtonText = "Відміна",
			string caption = "Підтвердіть дію");
		Task<Teacher> ShowTeacherForm(Teacher teacher = null);
		Task<Subject> ShowSubjectForm(Subject subject = null);
		Task<LoadItem> ShowLoadItemForm(LoadItem loadItem = null);
		Task<Group> ShowGroupForm(Group group = null);
		Task ShowSubjectImportPopup();
		Task<List<string>> ShowSubjectPickerPopup(
			string teacherName,
			List<string> subjects);
		Task ShowChangeDataStoragePopup(SettingsViewModel settingsViewModel);
		bool CanShowDialog { get; }
	}
}
