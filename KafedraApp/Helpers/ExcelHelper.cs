using KafedraApp.Attributes;
using KafedraApp.Extensions;
using KafedraApp.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace KafedraApp.Helpers
{
	public static class ExcelHelper
	{
		#region Fields

		private static Application _app;

		private static object[,] _data;

		private static List<string> _titles;

		#endregion

		#region Properties

		public static int DataStartRow = 3;

		public static char DataStartColumn = 'A';

		public static int HeaderRow = 1;

		#endregion

		#region Public methods

		public static List<Subject> GetSubjects(string filePath)
		{
			_app = new Application();

			var subjects = new List<Subject>();
			var workSheet = OpenWorkSheet(filePath);
			_titles = GetTitles(workSheet);
			var rowsCount = GetWorkSheetRowsCount(workSheet);

			string leftTopCell = $"{ DataStartColumn }{ DataStartRow }";

			string rightBottomCell =
				$"{ (char)(DataStartColumn + _titles.Count - 1) }{ DataStartRow + rowsCount - 1 }";

			_data = GetData(workSheet, leftTopCell, rightBottomCell);

			for (int i = DataStartRow; i < rowsCount; ++i)
			{
				var subject = GetItem<Subject>(i - DataStartRow);

				if (subject != null)
					subjects.Add(subject);
			}

			_app.Quit();

			return subjects;
		}

		#endregion

		#region Private methods

		private static Worksheet OpenWorkSheet(string filePath)
		{
			var workBook = _app.Workbooks.Open(filePath, 0, true, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
			var workSheet = (Worksheet)workBook.Sheets[1];
			return workSheet;
		}

		private static T GetItem<T>(int row)
		{
			var t = typeof(T);
			var props = t.GetProperties();
			var item = Activator.CreateInstance<T>();
			
			foreach (var prop in props)
			{
				var columnAttr = prop.GetCustomAttribute<ExcelColumnAttribute>();
				
				if (columnAttr != null)
				{
					var columnIndex = _titles.IndexOf(columnAttr.Column);

					if (columnIndex > -1)
						prop.SetValue(item, _data[row + 1, columnIndex + 1]);
				}
			}

			return item;
		}

		private static List<string> GetTitles(Worksheet workSheet)
		{
			var titles = new List<string>();

			for (char i = DataStartColumn; workSheet.GetValue(i, HeaderRow) != null; ++i)
			{
				titles.Add(workSheet.GetValue(i, HeaderRow));
			}

			return titles;
		}

		private static int GetWorkSheetRowsCount(Worksheet workSheet)
		{
			int rows = DataStartRow;

			while (workSheet.GetValue(DataStartColumn, rows++) != null) ;

			return rows;
		}

		private static object[,] GetData(Worksheet workSheet, string leftTopCell, string rightBottomCell)
		{
			var range = workSheet.get_Range(leftTopCell, rightBottomCell);
			var data = (object[,])range.Cells.Value;
			return data;
		}

		#endregion
	}
}
