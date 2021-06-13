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
		public int Course { get; set; }

		private int _semester;
		[ExcelColumn("Семестр")]
		public int Semester
		{
			get => _semester;
			set => _semester = CastSemesterTo1Or2(value);
		}

		[ExcelColumn("Кредити")]
		public double Credits { get; set; }

		public double TotalHours => Credits * HoursPerCredit;

		public int TotalClassroomHours => LectureHours + PracticalWorkHours + LaboratoryWorkHours;

		[ExcelColumn("Лекції")]
		public int LectureHours { get; set; }

		[ExcelColumn("Практичні")]
		public int PracticalWorkHours { get; set; }

		[ExcelColumn("Лабораторні")]
		public int LaboratoryWorkHours { get; set; }

		public FinalControlFormType FinalControlFormType { get; set; }

		private int _examSemester;
		[Obsolete]
		[ExcelColumn("Екзамен")]
		public int ExamSemester
		{
			get => _examSemester;
			set
			{
				if (value > 0
					&& FinalControlFormType == FinalControlFormType.Undefined)
				{
					FinalControlFormType = FinalControlFormType.Exam;
				}

				_examSemester = CastSemesterTo1Or2(value);
			}
		}

		private int _testSemester;
		[Obsolete]
		[ExcelColumn("Залік")]
		public int TestSemester
		{
			get => _testSemester;
			set
			{
				if (value > 0
					&& FinalControlFormType == FinalControlFormType.Undefined)
				{
					FinalControlFormType = FinalControlFormType.Test;
				}

				_testSemester = CastSemesterTo1Or2(value);
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

		private int CastSemesterTo1Or2(int semester)
		{
			return 2 - (semester % 2);
		}
	}
}
