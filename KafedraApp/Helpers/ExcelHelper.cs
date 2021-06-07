using KafedraApp.Attributes;
using KafedraApp.Extensions;
using KafedraApp.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

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

		public static string DataStartColumn = "A";

		public static int HeaderRow = 1;

		#endregion

		#region Delegates

		public delegate void ExcelLoadProgressChangedEventHandler(
			ExcelLoadProgressChangedEventArgs e);

		#endregion

		#region Events

		public static event ExcelLoadProgressChangedEventHandler LoadProgressChanged;
		public static event EventHandler LoadCompleted;

		#endregion

		#region Public Methods

		public static List<Subject> GetSubjects(string[] filePaths)
		{
			var subjects = new List<Subject>();
			var args = new ExcelLoadProgressChangedEventArgs
			{
				TotalSheets = filePaths.Length
			};

			foreach (var filePath in filePaths)
			{
				args.CurrentSheetName = filePath.Split('\\').Last();
				args.CurrentSheetLoadingProgress = 0;
				LoadProgressChanged?.Invoke(args);

				if (_app == null)
					_app = new Application();

				var workBook = _app.Workbooks.Open(filePath, 0, true, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
				var workSheet = (Worksheet)workBook.Sheets[1];

				_titles = GetTitles(workSheet);
				var rowsCount = GetWorkSheetRowsCount(workSheet);

				string leftTopCell = $"{ DataStartColumn }{ DataStartRow }";

				string rightBottomCell =
					$"{ IncCol(DataStartColumn, _titles.Count - 1) }{ DataStartRow + rowsCount - 1 }";

				_data = GetData(workSheet, leftTopCell, rightBottomCell);

				for (int i = DataStartRow; i < rowsCount - 1; ++i)
				{
					var subject = GetItem<Subject>(i - DataStartRow);

					if (subject != null)
						subjects.Add(subject);

					args.CurrentSheetLoadingProgress = (i - DataStartRow + 1) * 100 / (rowsCount - DataStartRow - 1);
					LoadProgressChanged?.Invoke(args);
				}

				++args.LoadedSheets;
				LoadProgressChanged?.Invoke(args);

				Marshal.ReleaseComObject(workSheet);
				workBook.Close(0);
				Marshal.ReleaseComObject(workBook);
			}

			_app.Quit();
			_app = null;
			LoadCompleted?.Invoke(null, null);
			return subjects;
		}

		#endregion

		#region Private Methods

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
					var column = _titles.FindIndex(x => Simplify(columnAttr.Column) == Simplify(x));

					if (column > -1)
					{
						var value = _data[row + 1, column + 1];

						if (value != null
							&& (prop.Name == nameof(Subject.Semester)
							|| prop.Name == nameof(Subject.ExamSemester)
							|| prop.Name == nameof(Subject.TestSemester)))
						{
							value = 2 - (double)value % 2;
						}

						prop.SetValue(item, value);
					}
				}
			}

			return item;
		}

		private static string Simplify(string str)
		{
			var res = Regex.Replace(str.ToLower(), @"\s+", "");
			return res;
		}

		private static List<string> GetTitles(Worksheet workSheet)
		{
			var titles = new List<string>();

			for (string i = DataStartColumn; ; i = IncCol(i))
			{
				var value = workSheet.GetValue(i, HeaderRow);

				if (string.IsNullOrWhiteSpace(value))
					break;

				titles.Add(value);
			}

			return titles;
		}

		private static int GetWorkSheetRowsCount(Worksheet workSheet)
		{
			int rows = DataStartRow;

			while (!string.IsNullOrWhiteSpace(workSheet.GetValue(DataStartColumn, rows++))) ;

			return rows;
		}

		private static object[,] GetData(
			Worksheet workSheet,
			string leftTopCell,
			string rightBottomCell)
		{
			var range = workSheet.get_Range(leftTopCell, rightBottomCell);
			var data = (object[,])range.Cells.Value;
			return data;
		}

		private static string IncCol(string col, int steps = 1)
		{
			if (col.Any(x => !char.IsUpper(x)))
				throw new ArgumentException();

			var chars = new StringBuilder(col);

			while (steps-- > 0)
			{
				int i = chars.Length - 1;

				while (i >= 0 && chars[i] == 'Z')
				{
					chars[i] = 'A';
					i--;
				}

				if (i == -1)
					chars.Insert(0, 'A');
				else
					chars[i]++;
			}

			return chars.ToString();
		}

		#endregion
	}
}
