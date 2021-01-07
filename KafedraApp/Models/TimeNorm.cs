using KafedraApp.Attributes;
using KafedraApp.Converters;
using System.ComponentModel;

namespace KafedraApp.Models
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum WorkTypes
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
	public enum DistributionTypes
	{
		[Description("на групу")]
		PerGroup,
		[Description("на студента")]
		PerStudent,
		[Description("на роботу")]
		PerWork
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum TimeNormCategories
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

		public WorkTypes WorkType { get; set; }

		public TimeNormCategories Category { get; set; }

		public DistributionTypes DistributionType { get; set; }

		private double _hours;
		public double Hours
		{
			get => _hours;
			set => SetProperty(ref _hours, value);
		}
	}
}
