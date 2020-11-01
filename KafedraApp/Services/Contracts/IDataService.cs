using KafedraApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KafedraApp.Services
{
	public interface IDataService
	{
		ObservableCollection<Subject> Subjects { get; set; }
		ObservableCollection<Teacher> Teachers { get; set; }
		ObservableCollection<AcademicStatusInfo> AcademicStatuses { get; set; }
		Task InitAsync();
	}
}
