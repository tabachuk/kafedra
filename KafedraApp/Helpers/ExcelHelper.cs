using KafedraApp.Extensions;
using KafedraApp.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;

namespace KafedraApp.Helpers
{
	public static class ExcelHelper
	{
		#region Fields

		private static Application _app;

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

			var workSheet = OpenWorkSheet(filePath);
			var titles = GetTitles(workSheet);
			var rowsCount = GetWorkSheetRowsCount(workSheet);

			string leftTopCell =
				$"{ DataStartColumn }{ DataStartRow }";
			string rightBottomCell =
				$"{ (char)(DataStartColumn + titles.Count - 1) }{ DataStartRow + rowsCount - 1 }";

			var data = GetData(workSheet, leftTopCell, rightBottomCell);

			_app.Quit();

			return null;
		}

		#endregion

		#region Private methods

		private static Worksheet OpenWorkSheet(string filePath)
		{
			var workBook = _app.Workbooks.Open(filePath, 0, true, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
			var workSheet = (Worksheet)workBook.Sheets[1];
			return workSheet;
		}

		private static T GetItem<T>(Worksheet workSheet, int row)
		{
			throw new NotImplementedException();
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
