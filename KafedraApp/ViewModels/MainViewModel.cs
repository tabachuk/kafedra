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

		private Sections _currentSection;
		public Sections CurrentSection
		{
			get => _currentSection;
			set => SetProperty(ref _currentSection, value);
		}

		#endregion

		#region Commands

		public ICommand SwitchSectionCommand { get; }

		#endregion

		#region Enum

		public enum Sections
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

		#region Constructors

		public MainViewModel()
		{
			_dataService = Container.Resolve<IDataService>();
			_dialogService = Container.Resolve<IDialogService>();

			SwitchSectionCommand = new DelegateCommand<Sections>(SwitchSection);

			App.Instance.ThemeChanged += (o, e) =>
				OnPropertyChanged(nameof(IsDarkMode));
		}

		#endregion

		#region Methods

		private async void SwitchSection(Sections section)
		{
			if (section == Sections.LoadDistribution)
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

			CurrentSection = section;
		}

		#endregion
	}
}
