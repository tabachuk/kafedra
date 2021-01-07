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
					WorkType = WorkTypes.Test,
					Category = TimeNormCategories.Test,
					Hours = 2,
					DistributionType = DistributionTypes.PerGroup
				},
				new TimeNorm
				{
					Name = "Модульна робота з фах. дисципліни",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Test,
					Hours = 0.1,
					DistributionType = DistributionTypes.PerWork
				},
				new TimeNorm
				{
					Name = "Усний",
					WorkType = WorkTypes.Exam,
					Category = TimeNormCategories.SemesterExam,
					Hours = 0.33,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Тестування",
					WorkType = WorkTypes.Exam,
					Category = TimeNormCategories.SemesterExam,
					Hours = 2,
					DistributionType = DistributionTypes.PerGroup
				},
				new TimeNorm
				{
					Name = "Рецензування рефератів при вступі",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Postgraduate,
					Hours = 3,
					DistributionType = DistributionTypes.PerGroup
				},
				new TimeNorm
				{
					Name = "Вступний іспит",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Postgraduate,
					Hours = 1,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Консультування здобувачів ст. доктора наук",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Postgraduate,
					Hours = 50,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Керівництво здобувачами ст. доктора філософії",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Postgraduate,
					Hours = 50,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Консультація",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.StateAttestation,
					Hours = 2,
					DistributionType = DistributionTypes.PerGroup
				},
				new TimeNorm
				{
					Name = "Проведення атестації",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.StateAttestation,
					Hours = 0.5,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Голова",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.BachalorWork,
					Hours = 0.5,
					DistributionType = DistributionTypes.PerWork
				},
				new TimeNorm
				{
					Name = "Екзаменатор",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.BachalorWork,
					Hours = 0.5,
					DistributionType = DistributionTypes.PerWork
				},
				new TimeNorm
				{
					Name = "Керівник/консультант",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.BachalorWork,
					Hours = 20,
					DistributionType = DistributionTypes.PerWork
				},
				new TimeNorm
				{
					Name = "Голова",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.MasterWork,
					Hours = 0.5,
					DistributionType = DistributionTypes.PerWork
				},
				new TimeNorm
				{
					Name = "Екзаменатор",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.MasterWork,
					Hours = 0.5,
					DistributionType = DistributionTypes.PerWork
				},
				new TimeNorm
				{
					Name = "Керівник/консультант",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.MasterWork,
					Hours = 28,
					DistributionType = DistributionTypes.PerWork
				},
				new TimeNorm
				{
					Name = "Курсова робота",
					WorkType = WorkTypes.CourseWork,
					Category = TimeNormCategories.Other,
					Hours = 3,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Контрольна робота",
					WorkType = WorkTypes.ControlWork,
					Category = TimeNormCategories.Other,
					Hours = 0.1,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Навчально-педагогічна",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Practice,
					Hours = 2,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Педагогічна (пропедевтична)",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Practice,
					Hours = 2,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Асистентська",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Practice,
					Hours = 2,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Переддипломна",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Practice,
					Hours = 2,
					DistributionType = DistributionTypes.PerStudent
				},
				new TimeNorm
				{
					Name = "Педагогічна",
					WorkType = WorkTypes.Other,
					Category = TimeNormCategories.Practice,
					Hours = 2,
					DistributionType = DistributionTypes.PerStudent
				},
			};

			File.WriteAllText("TimeNorms.json", timeNorms.ToJson());

			Console.Write("Press any key to close window...");
			Console.ReadKey();
		}
	}
}
