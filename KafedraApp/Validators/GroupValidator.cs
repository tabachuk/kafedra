using KafedraApp.Dtos;
using KafedraApp.Models;
using System.Globalization;

namespace KafedraApp.Validators
{
	public class GroupValidator : IGroupValidator
	{
		public Group Validate(GroupDto groupDto, out string error)
		{
			if (string.IsNullOrWhiteSpace(groupDto.Name))
			{
				error = "Вкажіть назву";
				return null;
			}

			if (string.IsNullOrWhiteSpace(groupDto.Specialty))
			{
				error = "Вкажіть спеціальність";
				return null;
			}

			if (string.IsNullOrWhiteSpace(groupDto.StudentsCountStr))
			{
				error = "Вкажіть кількість студентів";
				return null;
			}

			if (!int.TryParse(
				groupDto.StudentsCountStr,
				NumberStyles.Any,
				CultureInfo.InvariantCulture,
				out groupDto.StudentsCount))
			{
				error = "Невірний формат кількості студентів";
				return null;
			}

			if (groupDto.StudentsCount < 1 || groupDto.StudentsCount > 50)
			{
				error = "Група повинна містити від 1 до 50 студентів";
				return null;
			}

			Group group = CreateGroupFromDto(groupDto);

			error = null;
			return group;
		}

		private Group CreateGroupFromDto(GroupDto groupDto)
		{
			var group = new Group
			{
				Id = groupDto.Id,
				Name = groupDto.Name,
				Specialty = groupDto.Specialty,
				Course = groupDto.Course,
				SubgroupsCount = groupDto.SubgroupsCount,
				StudentsCount = groupDto.StudentsCount
			};

			return group;
		}
	}
}
