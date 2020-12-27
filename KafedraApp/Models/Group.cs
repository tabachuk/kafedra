using KafedraApp.Attributes;
using System;

namespace KafedraApp.Models
{
	[CollectionName("Groups")]
	public class Group : BaseModel, ICloneable
	{
		public string Name { get; set; }

		public string Specialty { get; set; }

		public double Course { get; set; }

		public int SubgroupsCount { get; set; }

		public int StudentsCount { get; set; }

		public object Clone()
		{
			return new Group
			{
				Id = Id,
				Name = Name,
				Specialty = Specialty,
				Course = Course,
				SubgroupsCount = SubgroupsCount,
				StudentsCount = StudentsCount
			};
		}
	}
}
