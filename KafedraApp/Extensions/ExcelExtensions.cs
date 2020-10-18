using Microsoft.Office.Interop.Excel;

namespace KafedraApp.Extensions
{
	public static class ExcelExtensions
	{
		public static string GetValue(this Worksheet workSheet, char column, int row)
		{
			return workSheet.Range[$"{ column }{ row }"].Value2;
		}
	}
}
