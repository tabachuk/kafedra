using KafedraApp.Models;
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
		bool CanShowDialog { get; }
	}
}
