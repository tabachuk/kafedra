using KafedraApp.Commands;
using KafedraApp.Helpers;
using KafedraApp.Properties;
using KafedraApp.Services;
using System.Linq;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class MainViewModel : BindableBase
	{
		#region Enum

		public enum Tab
		{
			Subjects,
			Teachers,
			Groups,
			AcademicStatuses,
			TimeNorms,
			LoadDistribution,
			Settings,
			Help
		};

		#endregion

		#region Fields

		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		#endregion

		#region Properties

		public bool IsDarkMode
		{
			get => App.Instance.SelectedTheme.ToLower() == "dark";
			set => App.Instance.SetTheme(value ? "Dark" : "Light");
		}

		public bool IsSideBarMinimized
		{
			get => Settings.Default.IsSideBarMinimized;
			set
			{
				Settings.Default.IsSideBarMinimized = value;
				OnPropertyChanged(nameof(IsSideBarMinimized));
			}
		}

		private Tab _currentTab;
		public Tab CurrentTab
		{
			get => _currentTab;
			set => SetProperty(ref _currentTab, value);
		}

		#endregion

		#region Commands

		public ICommand SwitchTabCommand { get; }

		#endregion

		#region Constructors

		public MainViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			SwitchTabCommand = new DelegateCommand<Tab>(SwitchTab);

			App.Instance.ThemeChanged += (o, e) =>
				OnPropertyChanged(nameof(IsDarkMode));
		}

		#endregion

		#region Methods

		private async void SwitchTab(Tab tab)
		{
			if (tab == Tab.LoadDistribution)
			{
				bool hasTeachersAndSubjects =
					_dataService.Teachers?.Any() == true
					&& _dataService.Subjects?.Any() == true;

				if (!hasTeachersAndSubjects)
				{
					await _dialogService.ShowError(
						"Для відкритття даної вкладки потрібно мати хоча б одного викладача та один предмет.",
						"Неможливо відкрити вкладку");
					return;
				}
			}

			CurrentTab = tab;
		}

		#endregion
	}
}
