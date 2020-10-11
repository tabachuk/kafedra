using KafedraApp.Converters;
using System.ComponentModel;

namespace KafedraApp.Enum
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum AcademicStatuses
	{
		[Description("Професор")]
		Professor,
		[Description("Доцент")]
		Docent,
		[Description("Старший викладач")]
		SeniorTeacher,
		[Description("Викладач")]
		Teacher
	}
}
