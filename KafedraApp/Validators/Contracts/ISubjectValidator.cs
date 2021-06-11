using KafedraApp.Dtos;
using KafedraApp.Models;

namespace KafedraApp.Validators
{
	public interface ISubjectValidator
	{
		Subject Validate(SubjectDto groupDto, out string error);
	}
}
