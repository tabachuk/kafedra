using KafedraApp.Dtos;
using KafedraApp.Models;

namespace KafedraApp.Validators
{
	public interface ITeacherValidator
	{
		Teacher Validate(TeacherDto loadItemDto, out string error);
	}
}
