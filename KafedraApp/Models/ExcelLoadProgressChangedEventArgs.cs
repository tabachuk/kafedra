using System;

namespace KafedraApp.Models
{
	public class ExcelLoadProgressChangedEventArgs : EventArgs
	{
		public int LoadedSheets { get; set; }

		public int TotalSheets { get; set; }

		public string CurrentSheetName { get; set; }

		public double CurrentSheetLoadingProgress { get; set; }
	}
}
