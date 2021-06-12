using KafedraApp.Dtos;
using KafedraApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafedraApp.Validators
{
	public class TeacherValidator : ITeacherValidator
	{
		public Teacher Validate(TeacherDto teacherDto, out string error)
		{
			if (string.IsNullOrWhiteSpace(teacherDto.LastName))
			{
				error = "Вкажіть прізвище";
				return null;
			}

			if (string.IsNullOrWhiteSpace(teacherDto.FirstName))
			{
				error = "Вкажіть ім'я";
				return null;
			}

			if (string.IsNullOrWhiteSpace(teacherDto.MiddleName))
			{
				error = "Вкажіть по батькові";
				return null;
			}

			if (string.IsNullOrWhiteSpace(teacherDto.RateStr))
			{
				error = "Вкажіть ставку";
				return null;
			}

			if (!float.TryParse(
				teacherDto.RateStr,
				NumberStyles.Any,
				CultureInfo.InvariantCulture,
				out teacherDto.Rate))
			{
				error = "Невірний формат ставки";
				return null;
			}

			if (teacherDto.Rate < 0 || teacherDto.Rate > 2)
			{
				error = "Вказана некоректна ставка";
				return null;
			}

			Teacher teacher = CreateTeacherFromDto(teacherDto);

			error = null;
			return teacher;
		}

		private Teacher CreateTeacherFromDto(TeacherDto teacherDto)
		{
			var teacher = new Teacher
			{
				Id = teacherDto.Id,
				LastName = teacherDto.LastName,
				FirstName = teacherDto.FirstName,
				MiddleName = teacherDto.MiddleName,
				AcademicStatus = teacherDto.AcademicStatus,
				Rate = teacherDto.Rate
			};

			if (teacherDto.SubjectsSpecializesIn?.Any() == true)
			{
				teacher.SubjectsSpecializesIn = new ObservableCollection<string>(teacherDto.SubjectsSpecializesIn);
			}

			if (teacherDto.LoadItems?.Any() == true)
			{
				teacher.LoadItems = new ObservableCollection<LoadItem>(teacherDto.LoadItems);
			}

			if (!string.IsNullOrEmpty(teacherDto.Id))
			{
				teacher.Id = teacherDto.Id;
			}

			return teacher;
		}
	}
}
