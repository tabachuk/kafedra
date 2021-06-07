using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace KafedraApp.ViewModels
{
	public class TerminalViewModel : ViewModelBase
	{
		#region Fields

		private readonly Command[] _commands;

		private int _prevRequestIndex;

		#endregion

		#region Properties

		private string _request;
		public string Request
		{
			get => _request;
			set => SetProperty(ref _request, value);
		}

		private string[] PreviousRequests =>
			Outputs.Where(x => x.StartsWith("> ")).Reverse().ToArray();

		private static readonly ObservableCollection<string> _outputs =
			new ObservableCollection<string>();
		public ObservableCollection<string> Outputs => _outputs;

		#endregion

		#region Events

		public event EventHandler Quitted;

		#endregion

		#region Constructors

		public TerminalViewModel()
		{
			_commands = new Command[]
			{
				new Command(new string[] { "commands", "help" }, 0, parameters =>
				{
					Output("Commands: " + string.Join(" ", _commands.Select(x => x.Calls[0])));
				}),
				new Command(new string[] { "clear" }, 0, parameters =>
				{
					ClearHistory();
				}),
				new Command(new string[] { "quit", "q", "close" }, 0, parameters =>
				{
					Quitted?.Invoke(this, null);
				}),
				new Command(new string[] { "gettheme" }, 0, parameters =>
				{
					Output($"Selected theme: { App.Instance.SelectedTheme }");
				}),
				new Command(new string[] { "settheme" }, 1, parameters =>
				{
					App.Instance.SetTheme(parameters[0]);
				})
			};
		}

		#endregion

		#region Methods

		private void Output(string message) => _outputs.Add(message);

		private void ClearHistory() => _outputs.Clear();

		public void ExecuteCommand()
		{
			if (string.IsNullOrWhiteSpace(Request))
				return;

			var request = Request.Trim()
				.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			Output("> " + Request);
			Request = "";

			var command = _commands.FirstOrDefault(x => x.Calls.Contains(request[0]));

			if (command == null)
				return;

			var parameters = request.Skip(1).ToArray();

			if (parameters.Length != command.ParametersCount)
			{
				Output($"Command '{ request[0] }' has " +
					(command.ParametersCount > 0 ?
						$"{ command.ParametersCount } parameters" :
						"no parameters"));
				return;
			}

			command.Execute(parameters);
		}

		public void PastePreviousRequest()
		{
			if (!PreviousRequests.Any())
				return;

			Request = PreviousRequests[_prevRequestIndex].Remove(0, 2);

			if (++_prevRequestIndex == PreviousRequests.Length)
				_prevRequestIndex = 0;
		}

		#endregion

		#region Command Implementation

		public class Command
		{
			public string[] Calls { get; set; }

			public int ParametersCount { get; set; }

			public Action<string[]> Action { get; set; }

			public Command(string[] calls, int parametersCount, Action<string[]> action)
			{
				Calls = calls;
				ParametersCount = parametersCount;
				Action = action;
			}

			public void Execute(string[] parameters = null)
			{
				Action?.Invoke(parameters);
			}
		}

		#endregion
	}
}
