using KafedraApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KafedraApp.Services
{
	public interface IDataService
	{
		ObservableCollection<Subject> Subjects { get; set; }
		ObservableCollection<Teacher> Teachers { get; set; }
		ObservableCollection<Group> Groups { get; set; }
		ObservableCollection<AcademicStatusInfo> AcademicStatuses { get; set; }
		ObservableCollection<TimeNorm> TimeNorms { get; set; }
		List<string> SubjectNames { get; }
		Task InitAsync();
		Task SaveSubjects();
		Task SaveTeachers();
		Task SaveGroups();
		List<LoadItem> GetLoadItems(IEnumerable<Subject> subjects = null);
	}
}
