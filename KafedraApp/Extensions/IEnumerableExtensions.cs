using KafedraApp.Models;
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
	}
}
