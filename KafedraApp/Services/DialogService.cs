using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KafedraApp.Windows;
using KafedraApp.Popups;
using KafedraApp.Models;
using System.Collections.Generic;

namespace KafedraApp.Services
{
	public class DialogService : IDialogService
	{
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
				MessageType = MessageTypes.Error,
				Message = message,
				Caption = caption,
				OKButtonText = "OK"
			};

			Push(popup);
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
				MessageType = MessageTypes.Info,
				Message = message,
				Caption = caption,
				OKButtonText = "OK"
			};

			Push(popup);
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
				MessageType = MessageTypes.Question,
				Message = message,
				Caption = caption,
				OKButtonText = OKButtonText,
				CancelButtonText = cancelButtonText,
				CloseIfBackgroundClicked = false
			};

			Push(popup);
			var result = await popup.Result;
			Pop(popup);
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

		private void Push(UIElement popup) =>
			MainView.Children.Add(popup);

		private void Pop(UIElement popup) =>
			MainView.Children.Remove(popup);

		#endregion
	}
}
