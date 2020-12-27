using KafedraApp.Converters;
using System.ComponentModel;

namespace KafedraApp.Models
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum LoadItemTypes
	{
		[Description("Лекції")]
		Lectures,
		[Description("Практичні")]
		PracticalWork,
		[Description("Лабораторні")]
		LaboratoryWork,
		[Description("Контрольні")]
		ControlWork,
		[Description("Курсова робота")]
		CourseWork,
		[Description("Дипломна робота")]
		DiplomaWork,
		[Description("ІНДЗ")]
		IndividualTasks,
		[Description("Залік")]
		Test,
		[Description("Екзамен")]
		Exam,
		[Description("Виробнича практика")]
		Internship,
		[Description("Асистентська практика")]
		AssistantPractice,
		[Description("Педагогічна практика")]
		PedagogicalPractice,
	}

	public class LoadItem : BaseModel
	{
		public string Subject { get; set; }

		public LoadItemTypes Type { get; set; }

		public double Hours { get; set; }

		public string Group { get; set; }

		public double Subgroup { get; set; }

		public double Semester { get; set; }
	}
}
