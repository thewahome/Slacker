using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slacker.Config
{
	class SlackerConfigSection :ConfigurationSection
	{
		[ConfigurationProperty("teams",
							   IsRequired=false)]
		public TeamConfigElementCollection Teams
		{
			get { return (TeamConfigElementCollection)base["teams"]; }
			set { base["teams"] = value; }
		}
	}
}
