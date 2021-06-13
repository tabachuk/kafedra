using KafedraApp.Dtos;
using KafedraApp.Models;
using System.Globalization;

namespace KafedraApp.Validators
{
	public class SubjectValidator : ISubjectValidator
	{
		public Subject Validate(SubjectDto subjectDto, out string error)
		{
			if (string.IsNullOrWhiteSpace(subjectDto.Name))
			{
				error = "Вкажіть назву";
				return null;
			}

			if (subjectDto.Course == 0)
			{
				error = "Вкажіть курс";
				return null;
			}

			if (subjectDto.Semester == 0)
			{
				error = "Вкажіть семестр";
				return null;
			}

			if (string.IsNullOrWhiteSpace(subjectDto.Specialty))
			{
				error = "Вкажіть спеціальність";
				return null;
			}

			if (string.IsNullOrWhiteSpace(subjectDto.CreditsStr))
			{
				error = "Вкажіть кількість кредитів";
				return null;
			}

			if (!double.TryParse(
				subjectDto.CreditsStr,
				NumberStyles.Any,
				CultureInfo.InvariantCulture,
				out subjectDto.Credits))
			{
				error = "Невірний формат кількості кредитів";
				return null;
			}

			if (subjectDto.Credits <= 0 && subjectDto.Credits > 10)
			{
				error = "Кількість кредитів не є коректною";
				return null;
			}

			if (string.IsNullOrWhiteSpace(subjectDto.LectureHoursStr))
			{
				error = "Вкажіть кількість лекцій";
				return null;
			}

			if (!int.TryParse(
				subjectDto.LectureHoursStr,
				NumberStyles.Any,
				CultureInfo.InvariantCulture,
				out subjectDto.LectureHours))
			{
				error = "Невірний формат кількості лекцій";
				return null;
			}

			if (subjectDto.LectureHours < 0 && subjectDto.LectureHours > 1000)
			{
				error = "Кількість лекцій не є коректною";
				return null;
			}

			if (string.IsNullOrWhiteSpace(subjectDto.PracticalWorkHoursStr))
			{
				error = "Вкажіть кількість практичних занять";
				return null;
			}

			if (!int.TryParse(
				subjectDto.PracticalWorkHoursStr,
				NumberStyles.Any,
				CultureInfo.InvariantCulture,
				out subjectDto.PracticalWorkHours))
			{
				error = "Невірний формат кількості практичних занять";
				return null;
			}

			if (subjectDto.PracticalWorkHours < 0 && subjectDto.PracticalWorkHours > 1000)
			{
				error = "Кількість практичних занять не є коректною";
				return null;
			}

			if (string.IsNullOrWhiteSpace(subjectDto.LaboratoryWorkHoursStr))
			{
				error = "Вкажіть кількість лабораторних занять";
				return null;
			}

			if (!int.TryParse(
				subjectDto.LaboratoryWorkHoursStr,
				NumberStyles.Any,
				CultureInfo.InvariantCulture,
				out subjectDto.LaboratoryWorkHours))
			{
				error = "Невірний формат кількості лабораторних занять";
				return null;
			}

			if (subjectDto.LaboratoryWorkHours < 0 && subjectDto.LaboratoryWorkHours > 1000)
			{
				error = "Кількість лабораторних занять не є коректною";
				return null;
			}

			Subject subject = CreateSubjectFromDto(subjectDto);

			error = null;
			return subject;
		}

		private Subject CreateSubjectFromDto(SubjectDto subjectDto)
		{
			var subject = new Subject
			{
				Id = subjectDto.Id,
				Name = subjectDto.Name,
				Specialty = subjectDto.Specialty,
				Credits = subjectDto.Credits,
				LectureHours = subjectDto.LectureHours,
				PracticalWorkHours = subjectDto.PracticalWorkHours,
				LaboratoryWorkHours = subjectDto.LaboratoryWorkHours,
				FinalControlFormType = subjectDto.FinalControlFormType
			};

			return subject;
		}
	}
}
