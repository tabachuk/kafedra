using KafedraApp.Extensions;
using KafedraApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Misc
{
	class Program
	{
		static void Main()
		{
			var timeNorms = new List<TimeNorm>
			{
				new TimeNorm
				{
					WorkType = WorkTypes.Advice,
					Hours = 2,
					DistributionType = DistributionTypes.PerGroup
				},
				new TimeNorm
				{
					WorkType = WorkTypes.Test,
					Hours = 2,
					DistributionType = DistributionTypes.PerGroup
				},
				new TimeNorm
				{
					WorkType = WorkTypes.Exam,
					Hours = 3,
					DistributionType = DistributionTypes.PerGroup
				},
				new TimeNorm
				{
					WorkType = WorkTypes.ControlWork,
					Hours = 0.1,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					WorkType = WorkTypes.CourseWork,
					Hours = 3,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					WorkType = WorkTypes.BachalorWork,
					Hours = 20,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					WorkType = WorkTypes.MasterWork,
					Hours = 28,
					DistributionType = DistributionTypes.PerStudent
				}
			};

			File.WriteAllText("TimeNorms.json", timeNorms.ToJson());

			Console.Write("Press any key to close window...");
			Console.ReadKey();
		}
	}
}
