using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slacker.Config;

namespace Slacker
{
	internal  class Globals
	{
		static Globals()
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			Settings = config.Sections["slacker"] as SlackerConfigSection ?? new SlackerConfigSection();
		}

		internal static SlackerConfigSection Settings;
	}
}
