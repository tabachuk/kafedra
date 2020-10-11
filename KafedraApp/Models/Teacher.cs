using KafedraApp.Attributes;
using KafedraApp.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KafedraApp.Models
{
	[CollectionName("Teachers")]
	public class Teacher : BaseModel, ICloneable
	{
		public string LastName { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public AcademicStatuses AcademicStatus { get; set; }

		public float Rate { get; set; }

		[JsonIgnore]
		public List<Subject> SubjectsSpecializesIn { get; set; }

		[JsonIgnore]
		public List<Subject> SubjectsTeaches { get; set; }

		public bool IsValid(out string error)
		{
			if (string.IsNullOrWhiteSpace(LastName))
			{
				error = "Вкажіть прізвище";
				return false;
			}

			if (string.IsNullOrWhiteSpace(FirstName))
			{
				error = "Вкажіть ім'я";
				return false;
			}

			if (string.IsNullOrWhiteSpace(MiddleName))
			{
				error = "Вкажіть по батькові";
				return false;
			}

			if (Rate < 0 || Rate > 2)
			{
				error = "Вказана ставка не є коректною";
				return false;
			}

			error = null;
			return true;
		}

		public object Clone()
		{
			return new Teacher()
			{
				Id = Id,
				LastName = LastName,
				FirstName = FirstName,
				MiddleName = MiddleName,
				AcademicStatus = AcademicStatus,
				Rate = Rate,
				SubjectsSpecializesIn = SubjectsSpecializesIn == null ?
					null : new List<Subject>(SubjectsSpecializesIn),
				SubjectsTeaches = SubjectsTeaches == null ?
					null : new List<Subject>(SubjectsTeaches)
			};
		}
	}
}
