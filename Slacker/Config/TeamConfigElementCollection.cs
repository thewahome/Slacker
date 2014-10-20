using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slacker.Config
{
	class TeamConfigElementCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new TeamConfigElement();
		}

		protected override ConfigurationElement CreateNewElement(string elementName)
		{
			return new TeamConfigElement() { Name = elementName };
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TeamConfigElement)element).Name;
		}

		public void Add(TeamConfigElement element)
		{
			BaseAdd(element);
		}

		public void Remove(TeamConfigElement element)
		{
			BaseRemove(element.Name);
		}
	}
}
