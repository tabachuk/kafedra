using KafedraApp.Commands;
using KafedraApp.Extensions;
using KafedraApp.Helpers;
using KafedraApp.Models;
using KafedraApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class GroupsViewModel : BindableBase
	{
		#region Fields

		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		#endregion

		#region Properties

		public ObservableCollection<Group> Groups => _dataService.Groups;

		public bool IsGroupsEmpty => Groups?.Any() != true;

		#endregion

		#region Commands

		public ICommand AddGroupCommand { get; }
		public ICommand EditGroupCommand { get; }
		public ICommand DeleteGroupCommand { get; }
		public ICommand ClearGroupsCommand { get; }
		public ICommand AutogenerateGroupsCommand { get; }

		#endregion

		#region Constructors

		public GroupsViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			AddGroupCommand = new DelegateCommand(AddGroup);
			EditGroupCommand = new DelegateCommand<Group>(EditGroup);
			DeleteGroupCommand = new DelegateCommand<Group>(DeleteGroup);
			ClearGroupsCommand = new DelegateCommand(ClearGroups);
			AutogenerateGroupsCommand = new DelegateCommand(AutogenerateGroups);

			Groups.CollectionChanged += OnGroupsChanged;
		}

		#endregion

		#region Methods

		private async void AddGroup()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var group = await _dialogService.ShowGroupForm();

			if (group != null)
			{
				Groups?.Add(group);
				await _dataService.SaveGroups();
			}

			IsBusy = false;
		}

		private async void EditGroup(Group group)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			group = await _dialogService.ShowGroupForm(group.Clone() as Group);

			if (group != null)
			{
				var id = Groups.IndexOf(Groups.GetById(group.Id));
				Groups[id] = group;
				await _dataService.SaveGroups();
			}

			IsBusy = false;
		}

		private async void DeleteGroup(Group group)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var res = await _dialogService.ShowQuestion(
				$"Ви дійсно бажаєте видалити { group.Name }?");

			if (res)
			{
				Groups.Remove(group);
				await _dataService.SaveGroups();
			}

			IsBusy = false;
		}

		private async void ClearGroups()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var res = await _dialogService.ShowQuestion(
				$"Ви дійсно бажаєте видалити всі групи?");

			if (res)
			{
				Groups.Clear();
				await _dataService.SaveGroups();
			}

			IsBusy = false;
		}

		private async void AutogenerateGroups()
		{
			if (IsBusy)
				return;
			IsBusy = true;

			if (!IsGroupsEmpty)
			{
				var res = await _dialogService.ShowQuestion(
					"Поточні групи будуть видалені. Бажаєте продовжити?",
					caption: "Попередження");

				if (res)
					Groups.Clear();
				else
					return;
			}

			var subjects = _dataService.Subjects;

			if (subjects.Count <= 0)
			{
				await _dialogService.ShowError(
					"Спершу додайте предмети");
				return;
			}

			var groups = subjects
				.Where(x => !string.IsNullOrEmpty(x.Specialty) && x.Course > 0)
				.Select(x => new { x.Specialty, x.Course })
				.Distinct()
				.Select(x => new Group
				{
					Name = $"{ x.Specialty }-{ x.Course }1",
					Specialty = x.Specialty,
					Course = x.Course,
					StudentsCount = 10,
					SubgroupsCount = 1
				});

			foreach (var group in groups)
			{
				Groups.Add(group);
			}

			await _dataService.SaveGroups();

			IsBusy = false;
		}

		#endregion

		#region Event Handlers

		private void OnGroupsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsGroupsEmpty));
		}

		#endregion
	}
}
