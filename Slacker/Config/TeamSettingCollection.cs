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
	public class TeamSettingCollection : ObservableCollection<TeamSetting>
	{
		public TeamSetting this[string name]
		{
			get { return this.SingleOrDefault(t => t.Name == name); }
		}
	}
}
