using KafedraApp.Dtos;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System.Globalization;
using System.Linq;

namespace KafedraApp.Validators
{
	public class LoadItemValidator : ILoadItemValidator
	{
		public bool RequiresSubject(LoadItemType loadItemType) =>
			loadItemType != LoadItemType.TermPapers
			&& loadItemType != LoadItemType.DiplomaWorks
			&& loadItemType != LoadItemType.AssistantPractice
			&& loadItemType != LoadItemType.PedagogicalPractice
			&& loadItemType != LoadItemType.ProductionPractice
			&& loadItemType != LoadItemType.PostgraduateGuidance;

		public LoadItem Validate(LoadItemDto loadItemDto, out string error)
		{
			if (RequiresSubject(loadItemDto.Type)
				&& string.IsNullOrWhiteSpace(loadItemDto.Subject))
			{
				error = "Вкажіть предмет";
				return null;
			}

			if (string.IsNullOrWhiteSpace(loadItemDto.HoursStr))
			{
				error = "Вкажіть кількість годин";
				return null;
			}

			if (!double.TryParse(
				loadItemDto.HoursStr,
				NumberStyles.Any,
				CultureInfo.InvariantCulture,
				out loadItemDto.Hours)
				|| loadItemDto.Hours <= 0)
			{
				error = "Вказана некоректна кількість годин";
				return null;
			}

			LoadItem loadItem = CreateLoadItemFromDto(loadItemDto);

			error = null;
			return loadItem;
		}

		private LoadItem CreateLoadItemFromDto(LoadItemDto loadItemDto)
		{
			var loadItem = new LoadItem
			{
				Type = loadItemDto.Type,
				Subject = loadItemDto.Subject,
				Hours = loadItemDto.Hours,
				Group = loadItemDto.Group,
				Semester = loadItemDto.Semester
			};

			if (!string.IsNullOrEmpty(loadItemDto.Id))
			{
				loadItem.Id = loadItemDto.Id;
			}

			if (string.IsNullOrWhiteSpace(loadItemDto.Subgroup)
				|| loadItemDto.Subgroup == LoadItemDto.NoSubgroupText)
			{
				loadItem.Subgroup = 0;
			}
			else
			{
				loadItem.Subgroup = double.Parse(loadItemDto.Subgroup);
			}

			if (!string.IsNullOrWhiteSpace(loadItemDto.Teacher)
				&& loadItemDto.Teacher != LoadItemDto.NoTeacherText)
			{
				var dataService = Container.Resolve<IDataService>();

				var teacher = dataService.Teachers
					.FirstOrDefault(x => x.LastNameAndInitials == loadItemDto.Teacher);

				loadItem.Teacher = teacher;
			}

			return loadItem;
		}
	}
}
