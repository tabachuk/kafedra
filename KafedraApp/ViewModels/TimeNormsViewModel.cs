using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System.Collections.ObjectModel;

namespace KafedraApp.ViewModels
{
	public class TimeNormsViewModel : BindableBase
	{
		#region Fields

		private readonly IDataService _dataService;

		#endregion

		#region Properties

		public ObservableCollection<TimeNorm> TimeNorms => _dataService.TimeNorms;

		#endregion

		#region Constructors

		public TimeNormsViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
		}

		#endregion
	}
}
