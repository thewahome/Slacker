using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slacker.Models
{
	class Channel
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

		public int UnreadCount
		{
			get;
			set;
		}
	}
}
