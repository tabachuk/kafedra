using KafedraApp.Attributes;
using KafedraApp.Converters;
using System.ComponentModel;

namespace KafedraApp.Models
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum WorkType
	{
		[Description("Консультація")]
		Advice,
		[Description("Залік")]
		Test,
		[Description("Екзамен")]
		Exam,
		[Description("Контрольна робота")]
		ControlWork,
		[Description("Курсова робота")]
		CourseWork,
		[Description("Інше")]
		Other,
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum DistributionType
	{
		[Description("на групу")]
		PerGroup,
		[Description("на студента")]
		PerStudent,
		[Description("на роботу")]
		PerWork
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum TimeNormCategory
	{
		[Description("Підсумковий контроль")]
		FinalControl,
		[Description("Семестровий екзамен")]
		SemesterExam,
		[Description("Аспірантура")]
		Postgraduate,
		[Description("Державна атестація")]
		StateAttestation,
		[Description("Практика")]
		Practice,
		[Description("Бакалаврська робота")]
		BachalorWork,
		[Description("Магістерська робота")]
		MasterWork,
		[Description("Інше")]
		Other
	}

	[CollectionName("TimeNorms")]
	public class TimeNorm : BaseModel
	{
		public string Name { get; set; }

		public WorkType WorkType { get; set; }

		public TimeNormCategory Category { get; set; }

		public DistributionType DistributionType { get; set; }

		private double _hours;
		public double Hours
		{
			get => _hours;
			set => SetProperty(ref _hours, value);
		}
	}
}
