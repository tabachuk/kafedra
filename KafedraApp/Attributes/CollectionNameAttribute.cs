using System;

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
