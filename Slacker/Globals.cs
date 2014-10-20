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
		internal static SlackerConfigSection Settings = ConfigurationManager.GetSection("slacker") as SlackerConfigSection ?? new SlackerConfigSection();
	}
}
