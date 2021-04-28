using KafedraApp.Attributes;
using KafedraApp.Converters;
using System;
using System.ComponentModel;

namespace KafedraApp.Models
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum FinalControlFormType
	{
		[Description("Відсутня")]
		Undefined,
		[Description("Екзамен")]
		Exam,
		[Description("Залік")]
		Test
	}

	[CollectionName("Subjects")]
	public class Subject : BaseModel, ICloneable
	{
		[ExcelColumn("Предмет")]
		public string Name { get; set; }

		[ExcelColumn("Спеціальність")]
		public string Specialty { get; set; }

		[ExcelColumn("Курс")]
		public double Course { get; set; }

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

		[ExcelColumn("ІНДЗ")]
		public double IndividualTasksHours { get; set; }

		public FinalControlFormType FinalControlFormType { get; set; }

		private double _examSemester;
		[Obsolete]
		[ExcelColumn("Екзамен")]
		public double ExamSemester
		{
			get => _examSemester;
			set
			{
				_examSemester = value;

				if (value > 0 && FinalControlFormType == FinalControlFormType.Undefined)
					FinalControlFormType = FinalControlFormType.Exam;
			}
		}

		private double _testSemester;
		[Obsolete]
		[ExcelColumn("Залік")]
		public double TestSemester
		{
			get => _testSemester;
			set
			{
				_testSemester = value;

				if (value > 0 && FinalControlFormType == FinalControlFormType.Undefined)
					FinalControlFormType = FinalControlFormType.Test;
			}
		}

		public object Clone()
		{
			return new Subject()
			{
				Id = Id,
				Name = Name,
				Specialty = Specialty,
				Course = Course,
				Semester = Semester,
				Credits = Credits,
				TotalHours = TotalHours,
				TotalClassroomHours = TotalClassroomHours,
				LectureHours = LectureHours,
				PracticalWorkHours = PracticalWorkHours,
				LaboratoryWorkHours = LaboratoryWorkHours,
				IndividualTasksHours = IndividualTasksHours,
				ExamSemester = ExamSemester,
				TestSemester = TestSemester,
				FinalControlFormType = FinalControlFormType
			};
		}
	}
}
