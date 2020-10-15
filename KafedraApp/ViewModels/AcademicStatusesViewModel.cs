using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;

namespace KafedraApp.ViewModels
{
	public class AcademicStatusesViewModel : BindableBase
	{
		#region Fields

		private readonly IDataService _dataService;

		#endregion

		#region Properties

		public ObservableCollection<AcademicStatusInfo> AcademicStatuses =>
			_dataService.AcademicStatuses;

		#endregion

		#region Constructors

		public AcademicStatusesViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
		}

		#endregion
	}
}
