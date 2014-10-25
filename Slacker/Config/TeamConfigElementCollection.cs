using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Slacker.Config
{
	class TeamConfigElementCollection : ConfigurationElementCollection
	{
		public TeamConfigElement this[int i]
		{
			get { return this.BaseGet(i) as TeamConfigElement; }
		}

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

		public ObservableCollection<TeamConfigElement> View
		{
			get
			{
				ObservableCollection<TeamConfigElement> view = new ObservableCollection<TeamConfigElement>();

				foreach (TeamConfigElement team in this)
					view.Add(team);

				view.CollectionChanged += (sender, args) =>
				{
					switch (args.Action)
					{
						case NotifyCollectionChangedAction.Add:
							foreach (var newItem in args.NewItems)
								this.Add((TeamConfigElement)newItem);
							break;
						case NotifyCollectionChangedAction.Remove:
							foreach (var oldItem in args.OldItems)
								this.Remove((TeamConfigElement)oldItem);
							break;
					}
				};

				return view;
			}
		}
	}
}
