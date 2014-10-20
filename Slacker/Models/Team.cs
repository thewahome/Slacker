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
		public string Name
		{
			get;
			set;
		}

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

		public override string ToString()
		{
			return string.Format("Team Name:{0}, Token:{1}, Channels:{2}",
								 this.Name,
								 this.Token,
								 this.Channels.Count);
		}
	}
}
