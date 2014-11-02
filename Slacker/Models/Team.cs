using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public ObservableCollection<Channel> Channels
		{
			get;
			private set;
		}

		public ObservableCollection<Group> Groups
		{
			get;
			private set;
		}

		public ObservableCollection<Chat> Chats
		{
			get;
			private set;
		}

		public string LatestTimestamp
		{
			get;
			set;
		}

		public Team()
		{
			this.Channels = new ObservableCollection<Channel>();
			this.Groups = new ObservableCollection<Group>();
			this.Chats = new ObservableCollection<Chat>();
		}

		public void MarkRead()
		{


			foreach (Channel channel in this.Channels)
				channel.HasUnread = false;

			foreach (Group group in this.Groups)
				group.HasUnread = false;

			foreach (Chat chat in this.Chats)
				chat.HasUnread = false;
		}

		public override string ToString()
		{
			return string.Format("Team Name:{0}, Token:{1}, Channels:{2}, Groups:{3}, Chats{4}, LatestTimestamp:{5}",
								 this.Name,
								 this.Token,
								 this.Channels.Count,
								 this.Groups.Count,
								 this.Chats.Count,
								 this.LatestTimestamp);
		}
	}
}
