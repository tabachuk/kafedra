using KafedraApp.Helpers;
using KafedraApp.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KafedraApp.Dtos
{
	public class TeacherDto : BindableBase
	{
		#region Properties

		public string Id { get; set; }

		private string _lastName;
		public string LastName
		{
			get => _lastName;
			set => SetProperty(ref _lastName, value);
		}

		private string _firstName;
		public string FirstName
		{
			get => _firstName;
			set => SetProperty(ref _firstName, value);
		}

		private string _middleName;
		public string MiddleName
		{
			get => _middleName;
			set => SetProperty(ref _middleName, value);
		}

		private AcademicStatus _academicStatus;
		public AcademicStatus AcademicStatus
		{
			get => _academicStatus;
			set => SetProperty(ref _academicStatus, value);
		}

		private string _rate;
		public string Rate
		{
			get => _rate?.Replace(',', '.');
			set => SetProperty(ref _rate, value);
		}

		public List<string> SubjectsSpecializesIn { get; set; }

		public List<LoadItem> LoadItems { get; set; }

		#endregion

		#region Constructors

		public TeacherDto(Teacher teacher = null)
		{
			if (teacher != null)
			{
				InitFromTeacher(teacher);
			}
		}

		public void InitFromTeacher(Teacher teacher)
		{
			Id = teacher.Id;
			LastName = teacher.LastName;
			FirstName = teacher.FirstName;
			MiddleName = teacher.MiddleName;
			AcademicStatus = teacher.AcademicStatus;
			Rate = teacher.Rate.ToString(CultureInfo.InvariantCulture);

			if (teacher.SubjectsSpecializesIn?.Any() == true)
			{
				SubjectsSpecializesIn = teacher.SubjectsSpecializesIn.ToList();
			}

			if (teacher.LoadItems?.Any() == true)
			{
				LoadItems = teacher.LoadItems.ToList();
			}
		}

		#endregion
	}
}
