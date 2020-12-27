using KafedraApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

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
		public double Course { get; set; }

		private double _subgroups;
		[ExcelColumn("Кількість підгруп")]
		public double Subgroups
		{
			get => _subgroups;
			set => _subgroups = value < 1 ? 1 : value;
		}

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
		public double PracticalWorkHours { get; set; }

		[ExcelColumn("Лабораторні")]
		public double LaboratoryWorkHours { get; set; }

		[ExcelColumn("Контрольна робота")]
		public double ControlWorkHours { get; set; }

		[ExcelColumn("Курсова")]
		public double CourseWorkHours { get; set; }

		[ExcelColumn("ІНДЗ")]
		public double IndividualTasksHours { get; set; }

		[ExcelColumn("Екзамен")]
		public double ExamHours { get; set; }

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
				PracticalWorkHours = PracticalWorkHours,
				LaboratoryWorkHours = LaboratoryWorkHours,
				ControlWorkHours = ControlWorkHours,
				CourseWorkHours = CourseWorkHours,
				IndividualTasksHours = IndividualTasksHours,
				ExamHours = ExamHours,
				TestHours = TestHours
			};
		}
	}
}
