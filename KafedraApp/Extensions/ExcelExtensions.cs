using Microsoft.Office.Interop.Excel;

namespace KafedraApp.Extensions
{
	public static class ExcelExtensions
	{
		public static string GetValue(this Worksheet workSheet, string col, int row)
		{
			var val = workSheet.Range[$"{ col }{ row }"].Value2 as string;
			return val?.Trim();
		}
	}
}
