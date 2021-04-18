using KafedraApp.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace KafedraApp.Popups
{
	public enum MessageType { Info, Error, Question };

	public partial class MessagePopup : INotifyPropertyChanged
	{
		#region Fields

		private readonly TaskCompletionSource<bool> _tcs
			= new TaskCompletionSource<bool>();

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register(
				nameof(Message),
				typeof(string),
				typeof(MessagePopup),
				new PropertyMetadata(null, null));

		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register(
				nameof(Caption),
				typeof(string),
				typeof(MessagePopup),
				new PropertyMetadata(null, null));

		public static readonly DependencyProperty OKButtonTextProperty =
			DependencyProperty.Register(
				nameof(OKButtonText),
				typeof(string),
				typeof(MessagePopup),
				new PropertyMetadata(null, null));

		public static readonly DependencyProperty CancelButtonTextProperty =
			DependencyProperty.Register(
				nameof(CancelButtonText),
				typeof(string),
				typeof(MessagePopup),
				new PropertyMetadata(null, null));

		public static readonly DependencyProperty MessageTypeProperty =
			DependencyProperty.Register(
				nameof(MessageType),
				typeof(MessageType),
				typeof(MessagePopup),
				new PropertyMetadata(MessageType.Info, null));

		public static readonly DependencyProperty CloseIfBackgroundClickedProperty =
			DependencyProperty.Register(
				nameof(CloseIfBackgroundClicked),
				typeof(bool),
				typeof(MessagePopup),
				new PropertyMetadata(true, null));

		#endregion

		#region Properties

		public Task<bool> Result => _tcs.Task;

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			private set => SetProperty(ref _isBusy, value);
		}

		public string Message
		{
			get => (string)GetValue(MessageProperty);
			set => SetValue(MessageProperty, value);
		}

		public string Caption
		{
			get => (string)GetValue(CaptionProperty);
			set => SetValue(CaptionProperty, value);
		}

		public string OKButtonText
		{
			get => (string)GetValue(OKButtonTextProperty);
			set => SetValue(OKButtonTextProperty, value);
		}

		public string CancelButtonText
		{
			get => (string)GetValue(CancelButtonTextProperty);
			set => SetValue(CancelButtonTextProperty, value);
		}

		public MessageType MessageType
		{
			get => (MessageType)GetValue(MessageTypeProperty);
			set => SetValue(MessageTypeProperty, value);
		}
		
		public bool CloseIfBackgroundClicked
		{
			get => (bool)GetValue(CloseIfBackgroundClickedProperty);
			set => SetValue(CloseIfBackgroundClickedProperty, value);
		}

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Commands

		public ICommand SetResultCommand { get; }

		#endregion

		#region Constructors

		public MessagePopup()
		{
			SetResultCommand = new DelegateCommand<bool>(SetResult);

			InitializeComponent();
		}

		#endregion

		#region Methods

		public override void EndInit()
		{
			base.EndInit();

			var anim = Resources["PushAnimation"] as Storyboard;
			BeginStoryboard(anim);
		}

		private void SetResult(bool result)
		{
			if (IsBusy)
				return;
			IsBusy = true;

			var anim = Resources["PopAnimation"] as Storyboard;

			anim.Completed += (o, e) =>
			{
				_tcs.SetResult(result);
				IsBusy = false;
			};

			BeginStoryboard(anim);
		}

		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(storage, value))
				return false;

			storage = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}

		#endregion
	}
}
