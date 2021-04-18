using KafedraApp.Commands;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class LoadDistributionViewModel : BindableBase
	{
		#region Constants

		private const int MaxLoadItemsToShow = 20;

		#endregion

		#region Fields

		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		#endregion

		#region Properties

		private Teacher _currentTeacher;
		public Teacher CurrentTeacher
		{
			get => _currentTeacher;
			set => SetProperty(ref _currentTeacher, value);
		}

		public ObservableCollection<Teacher> Teachers { get; }

		private ObservableCollection<LoadItem> _undistributedLoadItems;
		public ObservableCollection<LoadItem> UndistributedLoadItems
		{
			get => _undistributedLoadItems;
			set
			{
				if (_undistributedLoadItems != null)
					_undistributedLoadItems.CollectionChanged -= OnUndistributedLoadChanged;

				SetProperty(ref _undistributedLoadItems, value, OnUndistributedLoadItemsChanged);

				if (_undistributedLoadItems != null)
					_undistributedLoadItems.CollectionChanged += OnUndistributedLoadChanged;
			}
		}

		private ObservableCollection<LoadItem> _undistributedLoadItemsToShow;
		public ObservableCollection<LoadItem> UndistributedLoadItemsToShow
		{
			get => _undistributedLoadItemsToShow;
			set
			{
				if (_undistributedLoadItemsToShow != null)
					_undistributedLoadItemsToShow.CollectionChanged -= OnUndistributedLoadItemsToShowChanged;

				SetProperty(ref _undistributedLoadItemsToShow, value);
				OnPropertyChanged(nameof(IsLoadItemNotFound));

				if (_undistributedLoadItemsToShow != null)
					_undistributedLoadItemsToShow.CollectionChanged += OnUndistributedLoadItemsToShowChanged;
			}
		}

		public List<LoadItem> OrderedFilteredUndistributedLoadItems
		{
			get
			{
				if (UndistributedLoadItems?.Any() != true)
				{
					return UndistributedLoadItems?.ToList();
				}

				var filteredLoadItems = FilterLoadItems(UndistributedLoadItems, SearchText);
				var orderedFilteredLoadItems = OrderLoadItems(filteredLoadItems, SelectedSortingField);

				return orderedFilteredLoadItems.ToList();
			}
		}

		public double UndistributedLoadHours => UndistributedLoadItems?.Sum(x => x.Hours) ?? 0;

		public bool IsLoadItemNotFound => UndistributedLoadHours > 0 && _undistributedLoadItemsToShow?.Any() != true;

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set => SetProperty(ref _searchText, value, OnSearchTextChanged);
		}

		public List<string> SortingFields => new List<string>
		{
			"Предмет",
			"Тип роботи",
			"К-сть годин",
			"Група",
			"Семестр"
		};

		private string _selectedSortingField;
		public string SelectedSortingField
		{
			get => _selectedSortingField;
			set => SetProperty(ref _selectedSortingField, value, OnSelectedSortingFieldChanged);
		}

		#endregion

		#region Commands

		public ICommand SwitchTeacherCommand { get; }
		public ICommand AssignLoadCommand { get; }
		public ICommand UnassignLoadCommand { get; }
		public ICommand FormLoadCommand { get; }
		public ICommand ResetLoadCommand { get; }

		#endregion

		#region Constructors

		public LoadDistributionViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			SwitchTeacherCommand = new DelegateCommand<Teacher>(SwitchTeacher);
			AssignLoadCommand = new DelegateCommand<LoadItem>(AssignLoad);
			UnassignLoadCommand = new DelegateCommand<LoadItem>(UnassignLoad);
			FormLoadCommand = new DelegateCommand(FormLoad);
			ResetLoadCommand = new DelegateCommand(async () => await ResetLoad());

			Teachers = _dataService.Teachers;
			CurrentTeacher = _dataService.Teachers.FirstOrDefault();
			SelectedSortingField = SortingFields[0];

			var load = _dataService.GetLoadItems();
			var distributedLoad = Teachers?.Where(x => x?.LoadItems?.Count > 0)
				.SelectMany(x => x?.LoadItems).ToList();
			var notDistributedLoad = load.Except(distributedLoad).ToList();

			UndistributedLoadItems = new ObservableCollection<LoadItem>(notDistributedLoad);
			
			InitNotDistributedLoadToShow();
		}

		#endregion

		#region Methods

		private void SwitchTeacher(Teacher teacher)
		{
			CurrentTeacher = teacher;
		}

		private void AssignLoad(LoadItem loadItem)
		{
			if (CurrentTeacher.LoadItems == null)
				CurrentTeacher.LoadItems = new ObservableCollection<LoadItem>();

			CurrentTeacher.LoadItems.Add(loadItem);
			UndistributedLoadItems.Remove(loadItem);
		}

		private void UnassignLoad(LoadItem loadItem)
		{
			CurrentTeacher.LoadItems.Remove(loadItem);
			UndistributedLoadItems.Add(loadItem);
		}

		private void InitNotDistributedLoadToShow()
		{
			if (OrderedFilteredUndistributedLoadItems?.Any() == true)
			{
				UndistributedLoadItemsToShow = new ObservableCollection<LoadItem>(
					OrderedFilteredUndistributedLoadItems.Take(Math.Min(MaxLoadItemsToShow, OrderedFilteredUndistributedLoadItems.Count)));
			}
			else
			{
				UndistributedLoadItemsToShow = new ObservableCollection<LoadItem>();
			}
		}

		public void AddNotDistributedLoadItemToShow()
		{
			if (UndistributedLoadItemsToShow.Count >= OrderedFilteredUndistributedLoadItems.Count)
				return;

			UndistributedLoadItemsToShow
				.Add(OrderedFilteredUndistributedLoadItems[UndistributedLoadItemsToShow.Count]);
		}

		private async void FormLoad()
		{
			if (!await ResetLoad())
				return;

			foreach (var teacher in Teachers)
			{
				if (teacher.LoadItems == null)
					teacher.LoadItems = new ObservableCollection<LoadItem>();

				foreach (var subject in teacher.SubjectsSpecializesIn)
				{
					var loadItems = UndistributedLoadItems.Where(x => x.Subject == subject).ToList();

					for (int i = 0; i < loadItems.Count && teacher.LoadHours < teacher.RateHours; ++i)
					{
						var loadItem = loadItems[i];
						IEnumerable<LoadItem> relatedLoadItems;

						switch (loadItems[i].Type)
						{
							case LoadItemType.Lectures:
								teacher.LoadItems.Add(loadItem);
								UndistributedLoadItems.Remove(loadItem);

								relatedLoadItems = loadItems.Where(x =>
									x.Subject == loadItem.Subject
									&& x.Semester == loadItem.Semester
									&& (x.Type == LoadItemType.Exam || x.Type == LoadItemType.IndividualTasks));

								foreach (var relatedLoadItem in relatedLoadItems)
								{
									if (UndistributedLoadItems.Contains(relatedLoadItem))
									{
										UndistributedLoadItems.Remove(relatedLoadItem);
										teacher.LoadItems.Add(relatedLoadItem);
									}
								}
								break;
							case LoadItemType.LaboratoryWork:
							case LoadItemType.PracticalWork:
								teacher.LoadItems.Add(loadItem);
								UndistributedLoadItems.Remove(loadItem);

								relatedLoadItems = loadItems.Where(x =>
									x.Subject == loadItem.Subject
									&& x.Semester == loadItem.Semester
									&& (x.Type == LoadItemType.Test || x.Type == LoadItemType.ControlWork));

								foreach (var relatedLoadItem in relatedLoadItems)
								{
									if (UndistributedLoadItems.Contains(relatedLoadItem))
									{
										UndistributedLoadItems.Remove(relatedLoadItem);
										teacher.LoadItems.Add(relatedLoadItem);
									}
								}
								break;
						}
					}
				}
			}
		}

		private async Task<bool> ResetLoad()
		{
			if (!Teachers.Any(x => x.LoadItems?.Any() == true))
				return true;

			if (!await _dialogService.ShowQuestion("Ви дійсно бажаєте скинути поточний розподіл?"))
				return false;

			UndistributedLoadItems = new ObservableCollection<LoadItem>(_dataService.GetLoadItems());
			InitNotDistributedLoadToShow();

			foreach (var teacher in Teachers)
			{
				teacher.LoadItems = null;
			}

			return true;
		}

		private IEnumerable<LoadItem> FilterLoadItems(IEnumerable<LoadItem> loadItems, string searchText)
		{
			if (string.IsNullOrWhiteSpace(searchText))
			{
				return loadItems;
			}

			return loadItems.Where(x => x.Subject.ToLower().Contains(searchText.ToLower()));
		}

		private IEnumerable<LoadItem> OrderLoadItems(IEnumerable<LoadItem> loadItems, string byField)
		{
			switch (byField)
			{
				case "Предмет":
					return OrderBy(loadItems, x => x.Subject);
				case "Тип роботи":
					return OrderBy(loadItems, x => x.Type);
				case "К-сть годин":
					return OrderBy(loadItems, x => x.Hours);
				case "Група":
					return OrderBy(loadItems, x => x.Group);
				case "Семестр":
					return OrderBy(loadItems, x => x.Semester);
			}

			return loadItems;
		}

		private IEnumerable<LoadItem> OrderBy<TKey>(
			IEnumerable<LoadItem> loadItems,
			Func<LoadItem, TKey> keySelector,
			bool byDescending = false)
		{
			if (byDescending)
			{
				return loadItems.OrderByDescending(keySelector);
			}

			return loadItems.OrderBy(keySelector);
		}

		#endregion

		#region Event Handlers

		private void OnUndistributedLoadChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(UndistributedLoadHours));

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					var loadItem = e.NewItems[0] as LoadItem;
					var index = OrderedFilteredUndistributedLoadItems.IndexOf(loadItem);

					if (index >= 0 && index <= UndistributedLoadItemsToShow.Count)
					{
						UndistributedLoadItemsToShow.Insert(index, loadItem);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					loadItem = e.OldItems[0] as LoadItem;
					UndistributedLoadItemsToShow.Remove(loadItem);
					AddNotDistributedLoadItemToShow();
					break;
				case NotifyCollectionChangedAction.Replace:
					var newSubject = e.NewItems[0] as LoadItem;

					index = OrderedFilteredUndistributedLoadItems.IndexOf(newSubject);

					if (index >= 0 && index <= UndistributedLoadItemsToShow.Count)
					{
						UndistributedLoadItemsToShow.RemoveAt(index);
						UndistributedLoadItemsToShow.Insert(index, newSubject);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					UndistributedLoadItemsToShow.Clear();
					break;
			}
		}

		private void OnUndistributedLoadItemsToShowChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsLoadItemNotFound));
		}

		private void OnSearchTextChanged()
		{
			InitNotDistributedLoadToShow();
		}

		private void OnSelectedSortingFieldChanged()
		{
			InitNotDistributedLoadToShow();
		}

		private void OnUndistributedLoadItemsChanged()
		{
			OnPropertyChanged(nameof(UndistributedLoadHours));
		}

		#endregion
	}
}
