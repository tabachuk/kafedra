using KafedraApp.Helpers;
using KafedraApp.Models;
using System.Globalization;

namespace KafedraApp.Dtos
{
	public class LoadItemDto : BindableBase
	{
		#region Constants

		public const string NoSubgroupText = "Без розподілу на підгрупи";
		public const string NoTeacherText = "Не присвоювати";

		#endregion

		#region Properties

		public string Id { get; set; }

		private string _subject;
		public string Subject
		{
			get => _subject;
			set => SetProperty(ref _subject, value);
		}

		private LoadItemType _type;
		public LoadItemType Type
		{
			get => _type;
			set => SetProperty(ref _type, value);
		}

		public double Hours;
		private string _hoursStr;
		public string HoursStr
		{
			get => _hoursStr?.Replace(',', '.');
			set => SetProperty(ref _hoursStr, value);
		}

		private Group _group;
		public Group Group
		{
			get => _group;
			set => SetProperty(ref _group, value);
		}

		private string _subgroup = NoSubgroupText;
		public string Subgroup
		{
			get => _subgroup;
			set => SetProperty(ref _subgroup, value);
		}

		private double _semester = 1;
		public double Semester
		{
			get => _semester;
			set => SetProperty(ref _semester, value);
		}

		private string _teacher = NoTeacherText;
		public string Teacher
		{
			get => _teacher;
			set => SetProperty(ref _teacher, value);
		}

		#endregion

		#region Constructors

		public LoadItemDto(LoadItem loadItem = null)
		{
			if (loadItem != null)
			{
				InitFromLoadItem(loadItem);
			}
		}

		public void InitFromLoadItem(LoadItem loadItem)
		{
			Id = loadItem.Id;
			Subject = loadItem.Subject;
			Type = loadItem.Type;
			HoursStr = loadItem.Hours.ToString(CultureInfo.InvariantCulture);
			Group = loadItem.Group;
			Subgroup = loadItem.Subgroup == 0 ?
				NoSubgroupText : loadItem.Subgroup.ToString();
			Semester = loadItem.Semester;
			Teacher = loadItem.Teacher?.LastNameAndInitials ?? NoTeacherText;
		}

		#endregion
	}
}
