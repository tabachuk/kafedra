﻿using KafedraApp.Attributes;
using KafedraApp.Converters;
using System.ComponentModel;

namespace KafedraApp.Models
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum AcademicStatus
	{
		[Description("Викладач")]
		Teacher,
		[Description("Старший викладач")]
		SeniorTeacher,
		[Description("Доцент")]
		Docent,
		[Description("Професор")]
		Professor
	}

	[CollectionName("AcademicStatuses")]
	public class AcademicStatusInfo : BaseModel
	{
		public AcademicStatus Status { get; set; }

		private int _maxHours;
		public int MaxHours
		{
			get => _maxHours;
			set => SetProperty(ref _maxHours, value);
		}
	}
}
