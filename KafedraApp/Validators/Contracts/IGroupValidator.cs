using KafedraApp.Dtos;
using KafedraApp.Models;

namespace KafedraApp.Validators
{
	public interface IGroupValidator
	{
		Group Validate(GroupDto groupDto, out string error);
	}
}
