using KafedraApp.Attributes;
using KafedraApp.Helpers;
using KafedraApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace KafedraApp.Models
{
	[CollectionName("Teachers")]
	public class Teacher : BaseModel, ICloneable
	{
		#region Properties

		public string LastName { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public AcademicStatuses AcademicStatus { get; set; }

		public float Rate { get; set; }

		[JsonIgnore]
		public string FullName => $"{ LastName } { FirstName } { MiddleName }";

		[JsonIgnore]
		public string LastNameAndInitials => $"{ LastName } { FirstName[0] }. { MiddleName[0] }.";

		[JsonIgnore]
		public int MaxHours
		{
			get
			{
				var dataService = Container.Resolve<IDataService>();
				var maxHours = dataService.AcademicStatuses?
					.FirstOrDefault(x => x?.Status == AcademicStatus)?.MaxHours;
				return maxHours ?? 0;
			}
		}

		[JsonIgnore]
		public int RateHours => (int)Math.Ceiling(MaxHours * Rate);

		[JsonIgnore]
		public double LoadHours => Math.Ceiling(LoadItems?.Sum(x => x.Hours) ?? 0);

		private ObservableCollection<string> _subjectsSpecializesIn;
		public ObservableCollection<string> SubjectsSpecializesIn
		{
			get => _subjectsSpecializesIn;
			set
			{
				_subjectsSpecializesIn = value;
				OnPropertyChanged(nameof(SubjectsSpecializesInCount));
			}
		}

		[JsonIgnore]
		public int SubjectsSpecializesInCount => SubjectsSpecializesIn?.Count ?? 0;

		[JsonIgnore]
		public List<Subject> SubjectsTeaches { get; set; }

		private ObservableCollection<LoadItem> _loadItems;
		[JsonIgnore]
		public ObservableCollection<LoadItem> LoadItems
		{
			get => _loadItems;
			set
			{
				if (_loadItems != null)
					_loadItems.CollectionChanged -= OnLoadItemsChanged;

				_loadItems = value;
				OnPropertyChanged(nameof(LoadItems));
				OnPropertyChanged(nameof(LoadHours));

				if (_loadItems != null)
					_loadItems.CollectionChanged += OnLoadItemsChanged;
			}
		}

		#endregion

		#region Methods

		public bool IsValid(out string error)
		{
			if (string.IsNullOrWhiteSpace(LastName))
			{
				error = "Вкажіть прізвище";
				return false;
			}

			if (string.IsNullOrWhiteSpace(FirstName))
			{
				error = "Вкажіть ім'я";
				return false;
			}

			if (string.IsNullOrWhiteSpace(MiddleName))
			{
				error = "Вкажіть по батькові";
				return false;
			}

			if (Rate < 0 || Rate > 2)
			{
				error = "Вказана ставка не є коректною";
				return false;
			}

			error = null;
			return true;
		}

		public object Clone()
		{
			return new Teacher()
			{
				Id = Id,
				LastName = LastName,
				FirstName = FirstName,
				MiddleName = MiddleName,
				AcademicStatus = AcademicStatus,
				Rate = Rate,
				SubjectsSpecializesIn = SubjectsSpecializesIn == null ?
					null : new ObservableCollection<string>(SubjectsSpecializesIn),
				SubjectsTeaches = SubjectsTeaches == null ?
					null : new List<Subject>(SubjectsTeaches),
				LoadItems = LoadItems == null ?
					null : new ObservableCollection<LoadItem>(LoadItems)
			};
		}

		#endregion

		#region Event Handlers

		private void OnLoadItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(LoadHours));
		}

		#endregion
	}
}
