using KafedraApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KafedraApp.Extensions
{
	public static class IEnumerableExtensions
	{
		public static T GetById<T>(this IEnumerable<T> items, string id)
			where T : BaseModel
		{
			return items.FirstOrDefault(x => x.Id == id);
		}

		public static int GetOrderIndex<T>(this IEnumerable<T> items, T item) where T : IComparable
		{
			int i, count = items.Count();

			for (i = 0; i < count && items.ElementAt(i).CompareTo(item) < 0; ++i) ;

			return i;
		}
	}
}
