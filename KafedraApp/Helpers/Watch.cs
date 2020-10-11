using System;
using System.Diagnostics;

namespace KafedraApp.Helpers
{
	public static class Watch
	{
		private static readonly Stopwatch _watch = new Stopwatch();

		public static TimeSpan GetElapsed() => _watch.Elapsed;

		public static void Start() => _watch.Restart();

		public static void Stop(string prefix = "")
		{
			_watch.Stop();
			Debug.WriteLine(prefix + GetElapsedString());
		}

		private static string GetElapsedString()
		{
			var min = _watch.Elapsed.Minutes;
			var sec = _watch.Elapsed.Seconds;
			var mlsec = _watch.Elapsed.Milliseconds;

			if (min > 0)
				return $"{ min }m { sec }s { mlsec }ms";

			if (sec > 0)
				return $"{ sec }s { mlsec }ms";

			return $"{ mlsec }ms";
		}
	}
}
