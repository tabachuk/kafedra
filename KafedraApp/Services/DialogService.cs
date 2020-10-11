using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KafedraApp.Windows;
using KafedraApp.Popups;
using KafedraApp.Models;

namespace KafedraApp.Services
{
	public class DialogService : IDialogService
	{
		public async Task ShowError(
			string message,
			string caption = "Помилка")
		{
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
			var popup = new TeacherPopup(teacher);

			Push(popup);
			var result = await popup.Result;
			Pop(popup);
			return result;
		}

		private Panel MainView => MainWindow.Instance.Content as Panel;

		private void Push(UIElement popup) =>
			MainView.Children.Add(popup);

		private void Pop(UIElement popup) =>
			MainView.Children.Remove(popup);
	}
}
