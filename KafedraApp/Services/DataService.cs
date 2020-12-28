using KafedraApp.Attributes;
using KafedraApp.Extensions;
using KafedraApp.Helpers;
using KafedraApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KafedraApp.Services
{
	public class DataService : IDataService
	{
		#region Fields

		private readonly object _locker = new object();

		#endregion

		#region Properties

		public string DataFolder => "Data";

		public ObservableCollection<Subject> Subjects { get; set; }

		public ObservableCollection<Teacher> Teachers { get; set; }

		public ObservableCollection<Group> Groups { get; set; }

		public ObservableCollection<AcademicStatusInfo> AcademicStatuses { get; set; }

		public ObservableCollection<TimeNorm> TimeNorms { get; set; }

		public List<string> SubjectNames =>
			Subjects?
			.Select(x => x.Name)
			.Distinct()
			.Where(x => !x.Contains("робота") && !x.Contains("практика"))
			.OrderBy(x => x)
			.ToList();

		#endregion

		#region Private Methods

		private string GetPath<T>(string key) => $@"{ DataFolder }\{ key }.json";

		private T Read<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
			{
				var watch = new Watch().Start();
				var res = File.ReadAllText(path).FromJson<T>();
				Debug.Write($"'{ key }' has been read. ");
				watch.Stop("Duration: ");
				return res;
			}

			Debug.WriteLine($"Can't read '{ key }'.");
			return default;
		}

		private async Task<T> ReadAsync<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
			{
				var watch = new Watch().Start();
				var res = await Task.Run(() => File.ReadAllText(path).FromJson<T>());
				Debug.Write($"'{ key }' has been read. ");
				watch.Stop("Duration: ");
				return res;
			}

			Debug.WriteLine($"Can't read '{ key }'. File does not exist.");
			return default;
		}

		private List<T> ReadList<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
			{
				var watch = new Watch().Start();
				var res = File.ReadAllText(path).FromJson<List<T>>();
				Debug.Write($"'{ key }' has been read. { res.Count } items. ");
				watch.Stop("Duration: ");
				return res;
			}

			Debug.WriteLine($"Can't read '{ key }'. File does not exist.");
			return default;
		}

		private async Task<List<T>> ReadListAsync<T>(string key = null)
		{
			key = key ?? GetKey<T>();
			var path = GetPath<T>(key);

			if (File.Exists(path))
			{
				var watch = new Watch().Start();
				var res = await Task.Run(() => File.ReadAllText(path).FromJson<List<T>>());
				Debug.Write($"'{ key }' has been read. { res.Count } items. ");
				watch.Stop("Duration: ");
				return res;
			}

			Debug.WriteLine($"Can't read '{ key }'. File does not exist.");
			return default;
		}

		private void Write<T>(T data, string key = null)
		{
			lock (_locker)
			{
				var watch = new Watch().Start();
				key = key ?? GetKey<T>();

				if (!Directory.Exists(DataFolder))
					Directory.CreateDirectory(DataFolder);

				File.WriteAllText(GetPath<T>(key), data.ToJson());
				Debug.Write($"'{ key }' has been written. ");
				watch.Stop("Duration: ");
			}
		}

		private async Task WriteAsync<T>(T data, string key = null)
		{
			await Task.Run(() => Write(data, key));
		}

		private string GetKey<T>()
		{
			string key = null;

			if (typeof(T).GetInterfaces().Any(x => x.Name == "IEnumerable"))
			{
				key = typeof(T).GenericTypeArguments[0]
					.GetCustomAttributes(false)?
					.OfType<CollectionNameAttribute>()?
					.FirstOrDefault()?.CollectionName;
			}
			else if (typeof(T).BaseType == typeof(BaseModel))
			{
				key = typeof(T).GetCustomAttributes(false)?
					.OfType<CollectionNameAttribute>()?
					.FirstOrDefault()?.CollectionName;
			}

			return string.IsNullOrEmpty(key) ? typeof(T).Name : key;
		}

		private async Task InitSubjectsAsync()
		{
			var subjects = await ReadListAsync<Subject>();

			if (subjects?.Any() == true)
				Subjects = new ObservableCollection<Subject>(subjects);
			else
				Subjects = new ObservableCollection<Subject>();
		}

		private async Task InitTeachersAsync()
		{
			var teachers = await ReadListAsync<Teacher>();

			if (teachers?.Any() == true)
				Teachers = new ObservableCollection<Teacher>(teachers);
			else
				Teachers = new ObservableCollection<Teacher>();

			Teachers.CollectionChanged += OnTeachersChanged;
		}

		private async Task InitGroupsAsync()
		{
			var groups = await ReadListAsync<Group>();

			if (groups?.Any() == true)
				Groups = new ObservableCollection<Group>(groups);
			else
				Groups = new ObservableCollection<Group>();
		}

		private async Task InitMaxHoursAsync()
		{
			var academicStatuses = await ReadListAsync<AcademicStatusInfo>();

			if (academicStatuses == null)
				throw new FileNotFoundException(
					"Файл із вченими ступенями не знайдений. Перевірте його наявність в папці з даними");

			AcademicStatuses = new ObservableCollection<AcademicStatusInfo>(academicStatuses);
			AcademicStatuses.CollectionChanged += OnAcademicStatusesInfoChanged;

			foreach (var status in AcademicStatuses)
			{
				status.PropertyChanged += OnAcademicStatusInfoChanged;
			}
		}

		private async Task InitTimeNormsAsync()
		{
			var timeNorms = await ReadListAsync<TimeNorm>();

			if (timeNorms == null)
				throw new FileNotFoundException(
					"Файл із нормами часу не знайдений. Перевірте його наявність в папці з даними");

			TimeNorms = new ObservableCollection<TimeNorm>(timeNorms);
			TimeNorms.CollectionChanged += OnTimeNormsChanged;

			foreach (var timeNorm in TimeNorms)
			{
				timeNorm.PropertyChanged += OnTimeNormChanged;
			}
		}

		private List<LoadItem> GetLoadItemsByTeacher(Subject subject)
		{
			var groups = GetGroupsBySpecAndCourse(subject.Specialty, subject.Course);
			var loadItems = new List<LoadItem>();

			foreach (var group in groups)
			{
				if (subject.LectureHours > 0)
				{
					var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.Lectures,
						subject.LectureHours,
						group);

					loadItems.Add(loadItem);
				}

				if (subject.PracticalWorkHours > 0)
				{
					if (group.SubgroupsCount > 1)
					{
						for (int i = 1; i <= group.SubgroupsCount; ++i)
						{
							var loadItem = GetLoadItem(
								subject,
								LoadItemTypes.PracticalWork,
								subject.PracticalWorkHours,
								group,
								i);

							loadItems.Add(loadItem);
						}
					}
					else
					{
						var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.PracticalWork,
						subject.PracticalWorkHours,
						group);

						loadItems.Add(loadItem);
					}
				}

				if (subject.LaboratoryWorkHours > 0)
				{
					if (group.SubgroupsCount > 1)
					{
						for (int i = 1; i <= group.SubgroupsCount; ++i)
						{
							var loadItem = GetLoadItem(
								subject,
								LoadItemTypes.LaboratoryWork,
								subject.LaboratoryWorkHours,
								group,
								i);

							loadItems.Add(loadItem);
						}
					}
					else
					{
						var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.LaboratoryWork,
						subject.LaboratoryWorkHours,
						group);

					loadItems.Add(loadItem);
					}
				}

				if (subject.TestHours > 0)
				{
					var timeNorm = TimeNorms.FirstOrDefault(x => x.WorkType == WorkTypes.Test);

					if (timeNorm != null && timeNorm.Hours > 0)
					{
						var loadItem = GetLoadItem(
							subject,
							LoadItemTypes.Test,
							timeNorm.Hours,
							group);

						loadItems.Add(loadItem);
					}
				}

				if (subject.ExamHours > 0)
				{
					var timeNorm = TimeNorms.FirstOrDefault(x => x.WorkType == WorkTypes.Exam);

					if (timeNorm != null && timeNorm.Hours > 0)
					{
						var loadItem = GetLoadItem(
							subject,
							LoadItemTypes.Exam,
							timeNorm.Hours,
							group);

						loadItems.Add(loadItem);
					}
				}

				if (subject.IndividualTasksHours > 0)
				{
					var loadItem = GetLoadItem(
						subject,
						LoadItemTypes.IndividualTasks,
						subject.IndividualTasksHours,
						group);

					loadItems.Add(loadItem);
				}
			}

			return loadItems;
		}

		private LoadItem GetLoadItem(Subject subject, LoadItemTypes type, double hours, Group group, double subgroup = 0)
		{
			return new LoadItem
			{
				Subject = subject.Name,
				Semester = subject.Semester,
				Group = group.Name,
				Type = type,
				Hours = hours,
				Subgroup = subgroup
			};
		}

		private List<Subject> GetCathedraSubjects()
		{
			var assignedSubjects = Teachers.SelectMany(x => x.SubjectsSpecializesIn);
			var subjects = Subjects.Where(x => assignedSubjects.Contains(x.Name));
			return subjects.ToList();
		}

		#endregion

		#region Public Methods

		public async Task InitAsync()
		{
			await InitSubjectsAsync();
			await InitTeachersAsync();
			await InitGroupsAsync();
			await InitMaxHoursAsync();
			await InitTimeNormsAsync();
		}

		public async Task SaveTeachers()
		{
			await WriteAsync(Teachers);
		}

		public async Task SaveSubjects()
		{
			await WriteAsync(Subjects);
		}

		public async Task SaveGroups()
		{
			await WriteAsync(Groups);
		}

		public List<LoadItem> GetLoadItems(IEnumerable<Subject> subjects = null)
		{
			if (subjects?.Any() != true)
			{
				subjects = GetCathedraSubjects();
			}

			var loadItems = new List<LoadItem>();

			foreach (var subject in subjects)
			{
				loadItems.AddRange(GetLoadItemsByTeacher(subject));
			}

			return loadItems;
		}

		private List<Group> GetGroupsBySpecAndCourse(string specialty, double course)
		{
			var groups = Groups
				.Where(x => x.Specialty == specialty && x.Course == course).ToList();

			return groups;
		}

		#endregion

		#region Event Handlers

		private async void OnTeachersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			await WriteAsync(Teachers);
		}

		private async void OnAcademicStatusesInfoChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			await WriteAsync(AcademicStatuses);
		}

		private async void OnAcademicStatusInfoChanged(object sender, PropertyChangedEventArgs e)
		{
			await WriteAsync(AcademicStatuses);
		}

		private async void OnTimeNormsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			await WriteAsync(TimeNorms);
		}

		private async void OnTimeNormChanged(object sender, PropertyChangedEventArgs e)
		{
			await WriteAsync(TimeNorms);
		}

		#endregion
	}
}
