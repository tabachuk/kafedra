using System;
using System.Diagnostics;

namespace KafedraApp.Helpers
{
	public class Watch
	{
		private readonly Stopwatch _stopwatch = new Stopwatch();

		public TimeSpan GetElapsed() => _stopwatch.Elapsed;

		public Watch Start()
		{
			_stopwatch.Restart();
			return this;
		}

		public void Stop(string prefix = "")
		{
			_stopwatch.Stop();
			Debug.WriteLine(prefix + GetElapsedString());
		}

		private string GetElapsedString()
		{
			var min = _stopwatch.Elapsed.Minutes;
			var sec = _stopwatch.Elapsed.Seconds;
			var mlsec = _stopwatch.Elapsed.Milliseconds;

			if (min > 0)
				return $"{ min }m { sec }s { mlsec }ms";

			if (sec > 0)
				return $"{ sec }s { mlsec }ms";

			return $"{ mlsec }ms";
		}
	}
}
