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
		[Description("Бакалаврська робота")]
		BachalorWork,
		[Description("Магістерська робота")]
		MasterWork
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum DistributionTypes
	{
		[Description("на групу")]
		PerGroup,
		[Description("на студента")]
		PerStudent
	}

	[CollectionName("TimeNorms")]
	public class TimeNorm : BaseModel
	{
		public WorkTypes WorkType { get; set; }

		public DistributionTypes DistributionType { get; set; }

		private double _hours;
		public double Hours
		{
			get => _hours;
			set => SetProperty(ref _hours, value);
		}
	}
}
