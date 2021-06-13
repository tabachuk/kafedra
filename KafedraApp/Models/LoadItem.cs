using KafedraApp.Converters;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace KafedraApp.Models
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum LoadItemType
	{
		[Description("Лекції")]
		Lectures,
		[Description("Практичні заняття")]
		PracticalWorks,
		[Description("Лабораторні заняття")]
		LaboratoryWorks,
		[Description("Семінарські заняття")]
		SeminarWorks,
		[Description("Індивідуальні заняття")]
		IndividualWorks,
		[Description("Консультації протягом семестру")]
		Consultations,
		[Description("Екзаменаційні консультації")]
		ExamConsultations,
		[Description("Контрольні роботи")]
		ControlWorks,
		[Description("Реферати, аналітичні огляди, переклади")]
		AbstractsReviewsTranslations,
		[Description("Розрахункові, графічні, розрахунково-графічні роботи")]
		CalculationAndGraphicWorks,
		[Description("Курсові роботи")]
		TermPapers,
		[Description("Дипломні роботи")]
		DiplomaWorks,
		[Description("Залік")]
		Test,
		[Description("Екзамен")]
		Exam,
		[Description("Виробнича практика")]
		ProductionPractice,
		[Description("Асистентська практика")]
		AssistantPractice,
		[Description("Педагогічна практика")]
		PedagogicalPractice,
		[Description("Керівництво аспірантами")]
		PostgraduateGuidance,
	}

	public class LoadItem : BaseModel, IEquatable<LoadItem>
	{
		public string Subject { get; set; }

		public LoadItemType Type { get; set; }

		public double Hours { get; set; }

		public Group Group { get; set; }

		public int Subgroup { get; set; }

		public int Semester { get; set; }

		private Teacher _teacher;
		[JsonIgnore]
		public Teacher Teacher
		{
			get => _teacher;
			set => SetProperty(ref _teacher, value);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as LoadItem);
		}

		public bool Equals(LoadItem other)
		{
			return other != null
				&& Subject == other.Subject
				&& Type == other.Type
				&& Hours == other.Hours
				&& Group == other.Group
				&& Subgroup == other.Subgroup
				&& Semester == other.Semester;
		}

		public override int GetHashCode()
		{
			return (Subject, Type, Hours, Group, Subgroup, Semester).GetHashCode();
		}
	}
}
