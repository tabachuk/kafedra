using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafedraApp.Attributes
{
	public class CollectionNameAttribute : Attribute
	{
		public string CollectionName { get; set; }

        public CollectionNameAttribute(string name)
        {
            CollectionName = name;
        }
    }
}
