using System.Collections.Generic;

namespace KafedraApp.Models
{
	public class TimeNormsGroup
	{
		public TimeNormCategory Category { get; set; }

		public List<TimeNorm> TimeNorms { get; set; }

		public TimeNormsGroup(TimeNormCategory category, List<TimeNorm> timeNorms)
		{
			Category = category;
			TimeNorms = timeNorms;
		}
	}
}
