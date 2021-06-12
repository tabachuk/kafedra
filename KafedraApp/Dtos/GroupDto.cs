using KafedraApp.Helpers;
using KafedraApp.Models;

namespace KafedraApp.Dtos
{
	public class GroupDto : BindableBase
	{
		#region Properties

		public string Id { get; set; }

		private string _name;
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		private string _specialty;
		public string Specialty
		{
			get => _specialty;
			set => SetProperty(ref _specialty, value);
		}

		private double _course;
		public double Course
		{
			get => _course;
			set => SetProperty(ref _course, value);
		}

		private int _subgroupsCount;
		public int SubgroupsCount
		{
			get => _subgroupsCount;
			set => SetProperty(ref _subgroupsCount, value);
		}

		public int StudentsCount;
		private string _studentsCountStr;
		public string StudentsCountStr
		{
			get => _studentsCountStr;
			set => SetProperty(ref _studentsCountStr, value);
		}

		#endregion

		#region Constructors

		public GroupDto(Group group = null)
		{
			if (group != null)
			{
				InitFromGroup(group);
			}
		}

		public void InitFromGroup(Group group)
		{
			Id = group.Id;
			Name = group.Name;
			Specialty = group.Specialty;
			Course = group.Course;
			SubgroupsCount = group.SubgroupsCount;
			StudentsCountStr = group.StudentsCount.ToString();
		}

		#endregion
	}
}
