using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slacker.Models
{
	class Chat
	{
		public string ID
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool HasUnread
		{
			get;
			set;
		}

		public string LatestTimestamp
		{
			get;
			set;
		}
	}
}
