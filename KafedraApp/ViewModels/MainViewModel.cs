using KafedraApp.Commands;
using KafedraApp.Properties;
using System.Windows.Input;

namespace KafedraApp.ViewModels
{
	public class MainViewModel : BindableBase
	{
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
			set => SetProperty<Sections>(ref _currentSection, value);
		}

		#endregion

		#region Commands

		public ICommand SwitchSectionCommand { get; private set; }

		#endregion

		#region Enum

		public enum Sections
		{
			Subjects,
			Teachers,
			AcademicStatuses,
			Settings,
			Help
		};

		#endregion

		#region Constructors

		public MainViewModel()
		{
			CurrentSection = Sections.Subjects;

			SwitchSectionCommand = new DelegateCommand<Sections>(SwitchSection);

			App.Instance.ThemeChanged += (o, e) =>
				OnPropertyChanged(nameof(IsDarkMode));
		}

		#endregion

		#region Methods

		private void SwitchSection(Sections section)
		{
			CurrentSection = section;
		}

		#endregion
	}
}
