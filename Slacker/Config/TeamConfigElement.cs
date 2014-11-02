using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slacker.Config
{
	class TeamConfigElement :ConfigurationElement
	{
		[ConfigurationProperty("name",
							   IsKey = true,
							   IsRequired = true)]
		public string Name
		{
			get { return base["name"] as string; }
			set { base["name"] = value; }
		}


		[ConfigurationProperty("token",
							   IsRequired=true)]
		public string Token
		{
			get { return base["token"] as string; }
			set { base["token"] = value; }
		}

		[ConfigurationProperty("latest",
							   IsRequired = false)]
		public string Latest
		{
			get { return base["latest"] as string; }
			set { base["latest"] = value; }
		}
	}
}
