using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slacker.Core
{
	static class Utility
	{
		public static string DateTimeToTimestamp(DateTime dateTime)
		{
			TimeSpan t = dateTime - new DateTime(1970, 1, 1);

			return t.TotalSeconds.ToString();
		}
	}
}
