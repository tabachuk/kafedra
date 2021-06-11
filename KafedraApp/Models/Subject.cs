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
		public const double HoursPerCredit = 30;

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

		public double TotalHours => Credits * HoursPerCredit;

		public double TotalClassroomHours => LectureHours + PracticalWorkHours + LaboratoryWorkHours;

		[ExcelColumn("Лекції")]
		public double LectureHours { get; set; }

		[ExcelColumn("Практичні")]
		public double PracticalWorkHours { get; set; }

		[ExcelColumn("Лабораторні")]
		public double LaboratoryWorkHours { get; set; }

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
				LectureHours = LectureHours,
				PracticalWorkHours = PracticalWorkHours,
				LaboratoryWorkHours = LaboratoryWorkHours,
				ExamSemester = ExamSemester,
				TestSemester = TestSemester,
				FinalControlFormType = FinalControlFormType
			};
		}
	}
}
