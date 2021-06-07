using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System.Collections.ObjectModel;

namespace KafedraApp.ViewModels
{
	public class AcademicStatusesViewModel : ViewModelBase
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
