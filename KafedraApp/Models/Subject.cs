using KafedraApp.Attributes;
using System;

namespace KafedraApp.Models
{
	[CollectionName("Subjects")]
	public class Subject : BaseModel, ICloneable
	{
		[ExcelColumn("Предмет")]
		public string Name { get; set; }

		[ExcelColumn("Спеціальність")]
		public string Specialty { get; set; }

		[ExcelColumn("Курс")]
		public string Course { get; set; }

		[ExcelColumn("Кількість підгруп")]
		public double Subgroups { get; set; }

		[ExcelColumn("Семестр")]
		public double Semester { get; set; }

		[ExcelColumn("Кредити")]
		public double Credits { get; set; }

		[ExcelColumn("Всього")]
		public double TotalHours { get; set; }

		[ExcelColumn("Всього (аудиторних)")]
		public double TotalClassroomHours { get; set; }

		[ExcelColumn("Лекції")]
		public double LectureHours { get; set; }

		[ExcelColumn("Практичні")]
		public double PracticalHours { get; set; }

		[ExcelColumn("Лабораторні")]
		public double LaboratoryWorkHours { get; set; }

		[ExcelColumn("Контрольна робота")]
		public double ControlWorkHours { get; set; }

		[ExcelColumn("Курсова")]
		public double CourseWorkHours { get; set; }

		[ExcelColumn("ІНДЗ")]
		public double IndividualTaskHours { get; set; }

		[ExcelColumn("Екзамен")]
		public double ExaminationHours { get; set; }

		[ExcelColumn("Залік")]
		public double TestHours { get; set; }

		public object Clone()
		{
			return new Subject()
			{
				Id = Id,
				Name = Name,
				Specialty = Specialty,
				Course = Course,
				Subgroups = Subgroups,
				Semester = Semester,
				Credits = Credits,
				TotalHours = TotalHours,
				TotalClassroomHours = TotalClassroomHours,
				LectureHours = LectureHours,
				PracticalHours = PracticalHours,
				LaboratoryWorkHours = LaboratoryWorkHours,
				ControlWorkHours = ControlWorkHours,
				CourseWorkHours = CourseWorkHours,
				IndividualTaskHours = IndividualTaskHours,
				ExaminationHours = ExaminationHours,
				TestHours = TestHours
			};
		}
	}
}
