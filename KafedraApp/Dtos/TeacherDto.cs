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

		public string LastName { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public AcademicStatus AcademicStatus { get; set; }

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
