using System.Windows;
using System.Windows.Controls;

namespace KafedraApp.Controls
{
	public partial class IconButton : Button
	{
		#region Dependency properties

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register(
				nameof(Icon),
				typeof(char),
				typeof(IconButton),
				new PropertyMetadata((char)0xf0a6, null));

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(
				nameof(Text),
				typeof(string),
				typeof(IconButton),
				new PropertyMetadata(null, null));

		public static readonly DependencyProperty HasToggleProperty =
			DependencyProperty.Register(
				nameof(HasToggle),
				typeof(bool),
				typeof(IconButton),
				new PropertyMetadata(false, null));

		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register(
				nameof(IsChecked),
				typeof(bool),
				typeof(IconButton),
				new PropertyMetadata(false, null));

		#endregion

		#region Properties

		public char Icon
		{
			get => (char)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public bool HasToggle
		{
			get => (bool)GetValue(HasToggleProperty);
			set => SetValue(HasToggleProperty, value);
		}

		public bool IsChecked
		{
			get => (bool)GetValue(IsCheckedProperty);
			set => SetValue(IsCheckedProperty, value);
		}

		#endregion

		#region Constructors

		public IconButton()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		protected override void OnClick()
		{
			base.OnClick();
			IsChecked = !IsChecked;
		}

		#endregion
	}
}
