using System.Collections.Generic;

namespace KafedraApp.Models
{
	public class TimeNormsGroup
	{
		public TimeNormCategories Category { get; set; }

		public List<TimeNorm> TimeNorms { get; set; }

		public TimeNormsGroup(TimeNormCategories category, List<TimeNorm> timeNorms)
		{
			Category = category;
			TimeNorms = timeNorms;
		}
	}
}
