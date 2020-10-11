using KafedraApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KafedraApp.Services.Contracts
{
	public interface IDataService
	{
		ObservableCollection<Teacher> Teachers { get; set; }
		Task InitAsync();
		T Read<T>(string key = null);
		Task<T> ReadAsync<T>(string key = null);
		List<T> ReadList<T>(string key = null);
		Task<List<T>> ReadListAsync<T>(string key = null);
		void Write<T>(T data, string key = null);
		Task WriteAsync<T>(T data, string key = null);
	}
}
