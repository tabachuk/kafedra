using KafedraApp.Helpers;
using KafedraApp.Models;
using System.Globalization;

namespace KafedraApp.Dtos
{
	public class SubjectDto : BindableBase
	{
		#region Properties

		public string Id { get; set; }

		private string _name;
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		private string _specialty;
		public string Specialty
		{
			get => _specialty;
			set => SetProperty(ref _specialty, value);
		}

		private double _course;
		public double Course
		{
			get => _course;
			set => SetProperty(ref _course, value);
		}

		private double _semester;
		public double Semester
		{
			get => _semester;
			set => SetProperty(ref _semester, value);
		}

		public double Credits;
		private string _creditsStr;
		public string CreditsStr
		{
			get => _creditsStr?.Replace(',', '.');
			set => SetProperty(ref _creditsStr, value);
		}

		public double LectureHours;
		private string _lectureHoursStr;
		public string LectureHoursStr
		{
			get => _lectureHoursStr?.Replace(',', '.');
			set => SetProperty(ref _lectureHoursStr, value);
		}

		public double PracticalWorkHours;
		private string _practicalWorkHoursStr;
		public string PracticalWorkHoursStr
		{
			get => _practicalWorkHoursStr?.Replace(',', '.');
			set => SetProperty(ref _practicalWorkHoursStr, value);
		}

		public double LaboratoryWorkHours;
		private string _laboratoryWorkHoursStr;
		public string LaboratoryWorkHoursStr
		{
			get => _laboratoryWorkHoursStr?.Replace(',', '.');
			set => SetProperty(ref _laboratoryWorkHoursStr, value);
		}

		private FinalControlFormType _finalControlFormType;
		public FinalControlFormType FinalControlFormType
		{
			get => _finalControlFormType;
			set => SetProperty(ref _finalControlFormType, value);
		}

		#endregion

		#region Constructors

		public SubjectDto(Subject subject = null)
		{
			if (subject != null)
			{
				InitFromSubject(subject);
			}
		}

		public void InitFromSubject(Subject subject)
		{
			Id = subject.Id;
			Name = subject.Name;
			Specialty = subject.Specialty;
			Course = subject.Course;
			Semester = subject.Semester;
			CreditsStr = subject.Credits.ToString(CultureInfo.InvariantCulture);
			LectureHoursStr = subject.LectureHours.ToString(CultureInfo.InvariantCulture);
			PracticalWorkHoursStr = subject.PracticalWorkHours.ToString(CultureInfo.InvariantCulture);
			LaboratoryWorkHoursStr = subject.LaboratoryWorkHours.ToString(CultureInfo.InvariantCulture);
			FinalControlFormType = subject.FinalControlFormType;
		}

		#endregion
	}
}
