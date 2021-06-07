using KafedraApp.Dtos;
using KafedraApp.Models;

namespace KafedraApp.Validators
{
	public interface ILoadItemValidator
	{
		bool RequiresSubject(LoadItemType loadItemType);
		LoadItem Validate(LoadItemDto loadItemDto, out string error);
	}
}
