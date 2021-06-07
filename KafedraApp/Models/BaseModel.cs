using KafedraApp.Helpers;
using Newtonsoft.Json;
using System;

namespace KafedraApp.Models
{
	public abstract class BaseModel : BindableBase
    {
        [JsonProperty(Order = -int.MaxValue)]
        public string Id { get; set; }
        
        public BaseModel()
        {
            Id = Guid.NewGuid().ToString();
        }
	}
}
