using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KafedraApp.Windows;
using KafedraApp.Popups;
using KafedraApp.Models;
using System.Collections.Generic;
using NLog;
using KafedraApp.ViewModels;

namespace KafedraApp.Services
{
	public class DialogService : IDialogService
	{
		#region Fields

		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		#endregion

		#region Properties

		private Panel MainView => MainWindow.Instance.Content as Panel;

		public bool CanShowDialog => MainWindow.Instance?.Content != null;

		#endregion

		#region Methods

		public async Task ShowError(
			string message,
			string caption = "Помилка")
		{
			if (!CanShowDialog)
				return;

			var popup = new MessagePopup
			{
				MessageType = MessageType.Error,
				Message = message,
				Caption = caption,
				OKButtonText = "OK"
			};

			Push(popup);
			_logger.Info("Error popup showed. Caption: '{0}'. Message: '{1}'.", caption, message);
			await popup.Result;
			Pop(popup);
		}

		public async Task ShowInfo(
			string message,
			string caption = "Інформація")
		{
			if (!CanShowDialog)
				return;

			var popup = new MessagePopup
			{
				MessageType = MessageType.Info,
				Message = message,
				Caption = caption,
				OKButtonText = "OK"
			};

			Push(popup);
			_logger.Info("Info popup showed. Caption: '{0}'. Message: '{1}'.", caption, message);
			await popup.Result;
			Pop(popup);
		}

		public async Task<bool> ShowQuestion(
			string message,
			string OKButtonText = "ОК",
			string cancelButtonText = "Відміна",
			string caption = "Підтвердіть дію")
		{
			if (!CanShowDialog)
				return false;

			var popup = new MessagePopup
			{
				MessageType = MessageType.Question,
				Message = message,
				Caption = caption,
				OKButtonText = OKButtonText,
				CancelButtonText = cancelButtonText,
				CloseIfBackgroundClicked = false
			};

			Push(popup);
			_logger.Info("Question popup showed. Caption: '{0}'. Message: '{1}'. Options: '{2}'/'{3}'.", caption, message, OKButtonText, cancelButtonText);
			var result = await popup.Result;
			Pop(popup);
			_logger.Info("User has chosen '{0}'.", result? OKButtonText : cancelButtonText);
			return result;
		}

		public async Task<Teacher> ShowTeacherForm(Teacher teacher = null)
		{
			if (!CanShowDialog)
				return null;

			var popup = new TeacherPopup(teacher);

			Push(popup);
			var result = await popup.Result;
			Pop(popup);
			return result;
		}

		public async Task<Subject> ShowSubjectForm(Subject subject = null)
		{
			if (!CanShowDialog)
				return null;

			var popup = new SubjectPopup(subject);

			Push(popup);
			var result = await popup.Result;
			Pop(popup);
			return result;
		}

		public async Task<LoadItem> ShowLoadItemForm(LoadItem loadItem = null)
		{
			if (!CanShowDialog)
				return null;

			var popup = new LoadItemPopup(loadItem);

			Push(popup);
			var result = await popup.Result;
			Pop(popup);
			return result;
		}

		public async Task<Group> ShowGroupForm(Group group = null)
		{
			if (!CanShowDialog)
				return null;

			var popup = new GroupPopup(group);

			Push(popup);
			var result = await popup.Result;
			Pop(popup);
			return result;
		}

		public async Task ShowSubjectImportPopup()
		{
			if (!CanShowDialog)
				return;

			var popup = new SubjectImportPopup();

			Push(popup);
			await popup.Result;
			Pop(popup);
		}

		public async Task<List<string>> ShowSubjectPickerPopup(
			string teacherName,
			List<string> subjects)
		{
			if (!CanShowDialog)
				return null;

			var popup = new SubjectPickerPopup(teacherName, subjects);

			Push(popup);
			var res = await popup.Result;
			Pop(popup);

			return res;
		}

		public async Task ShowChangeDataStoragePopup(SettingsViewModel settingsViewModel)
		{
			if (!CanShowDialog)
				return;

			var popup = new ChangeDataStoragePopup
			{
				DataContext = settingsViewModel
			};

			Push(popup);
			await popup.Result;
			Pop(popup);
		}

		private void Push(UIElement popup) =>
			MainView.Children.Add(popup);

		private void Pop(UIElement popup) =>
			MainView.Children.Remove(popup);

		#endregion
	}
}
