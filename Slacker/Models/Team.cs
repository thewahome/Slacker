using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slacker.Models
{
	/// <summary>
	/// Slack Team
	/// </summary>
	class Team
	{
		/// <summary>
		/// Gets or sets the token.
		/// </summary>
		public string Token
		{
			get;
			set;
		}

		public List<Channel> Channels
		{
			get;
			private set;
		}

		public Team()
		{
			this.Channels = new List<Channel>();
		}
	}
}
