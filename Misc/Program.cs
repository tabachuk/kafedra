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
					Name = "Залік",
					WorkType = WorkType.Test,
					Category = TimeNormCategory.Test,
					Hours = 2,
					DistributionType = DistributionType.PerGroup
				},
				new TimeNorm
				{
					Name = "Модульна робота з фах. дисципліни",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Test,
					Hours = 0.1,
					DistributionType = DistributionType.PerWork
				},
				new TimeNorm
				{
					Name = "Усний",
					WorkType = WorkType.Exam,
					Category = TimeNormCategory.SemesterExam,
					Hours = 0.33,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Тестування",
					WorkType = WorkType.Exam,
					Category = TimeNormCategory.SemesterExam,
					Hours = 2,
					DistributionType = DistributionType.PerGroup
				},
				new TimeNorm
				{
					Name = "Рецензування рефератів при вступі",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Postgraduate,
					Hours = 3,
					DistributionType = DistributionType.PerGroup
				},
				new TimeNorm
				{
					Name = "Вступний іспит",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Postgraduate,
					Hours = 1,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Консультування здобувачів ст. доктора наук",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Postgraduate,
					Hours = 50,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Керівництво здобувачами ст. доктора філософії",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Postgraduate,
					Hours = 50,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Консультація",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.StateAttestation,
					Hours = 2,
					DistributionType = DistributionType.PerGroup
				},
				new TimeNorm
				{
					Name = "Проведення атестації",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.StateAttestation,
					Hours = 0.5,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Голова",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.BachalorWork,
					Hours = 0.5,
					DistributionType = DistributionType.PerWork
				},
				new TimeNorm
				{
					Name = "Екзаменатор",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.BachalorWork,
					Hours = 0.5,
					DistributionType = DistributionType.PerWork
				},
				new TimeNorm
				{
					Name = "Керівник/консультант",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.BachalorWork,
					Hours = 20,
					DistributionType = DistributionType.PerWork
				},
				new TimeNorm
				{
					Name = "Голова",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.MasterWork,
					Hours = 0.5,
					DistributionType = DistributionType.PerWork
				},
				new TimeNorm
				{
					Name = "Екзаменатор",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.MasterWork,
					Hours = 0.5,
					DistributionType = DistributionType.PerWork
				},
				new TimeNorm
				{
					Name = "Керівник/консультант",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.MasterWork,
					Hours = 28,
					DistributionType = DistributionType.PerWork
				},
				new TimeNorm
				{
					Name = "Курсова робота",
					WorkType = WorkType.CourseWork,
					Category = TimeNormCategory.Other,
					Hours = 3,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Контрольна робота",
					WorkType = WorkType.ControlWork,
					Category = TimeNormCategory.Other,
					Hours = 0.1,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Навчально-педагогічна",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Practice,
					Hours = 2,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Педагогічна (пропедевтична)",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Practice,
					Hours = 2,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Асистентська",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Practice,
					Hours = 2,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Переддипломна",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Practice,
					Hours = 2,
					DistributionType = DistributionType.PerStudent
				},
				new TimeNorm
				{
					Name = "Педагогічна",
					WorkType = WorkType.Other,
					Category = TimeNormCategory.Practice,
					Hours = 2,
					DistributionType = DistributionType.PerStudent
				},
			};

			File.WriteAllText("TimeNorms.json", timeNorms.ToJson());

			Console.Write("Press any key to close window...");
			Console.ReadKey();
		}
	}
}
