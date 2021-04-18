using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KafedraApp.ViewModels
{
	public class TimeNormsViewModel : BindableBase
	{
		#region Fields

		private readonly IDataService _dataService;

		#endregion

		#region Properties

		private ObservableCollection<TimeNorm> TimeNorms { get; }

		public ObservableCollection<TimeNormsGroup> TimeNormsGroups
		{
			get
			{
				var groups = new List<TimeNormsGroup>();

				foreach (var value in Enum.GetValues(typeof(TimeNormCategory)))
				{
					var category = (TimeNormCategory)value;
					var timeNorms = TimeNorms.Where(x => x.Category == category).ToList();
					groups.Add(new TimeNormsGroup(category, timeNorms));
				}

				return new ObservableCollection<TimeNormsGroup>(groups);
			}
		}

		#endregion

		#region Constructors

		public TimeNormsViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			TimeNorms = _dataService.TimeNorms;
		}

		#endregion
	}
}
