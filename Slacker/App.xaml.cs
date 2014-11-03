using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slacker.Config;
using Slacker.Core;
using Slacker.Models;
using Slacker.Properties;

namespace Slacker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : System.Windows.Application
	{
		#region Properties

		internal TaskbarIcon NotifyIcon
		{
			get;
			private set;
		}

		internal Timer SyncTimer
		{
			get;
			private set;
		}

		internal ObservableCollection<Team> Teams
		{
			get;
			private set;
		}

		#endregion

		#region Constructors

		protected App()
		{
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			this.Teams = new ObservableCollection<Team>();
		}

		#endregion

		#region Methods

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.InitialCommands();
			this.InitialTeams();
			this.InitialSync();
			this.InitialNotifyIcon();
		}

		public void InitialTeams()
		{
			if (this.SyncTimer != null)
				this.SyncTimer.Stop();

			this.Teams.Clear();

			foreach (TeamSetting teamConfig in Settings.Default.Teams)
			{
				Team team = new Team()
				{
					Name = teamConfig.Name,
					Token = teamConfig.Token
				};

				this.Teams.Add(team);
			}
		}

		public void InitialSync()
		{
			if (this.SyncTimer != null)
				this.SyncTimer.Stop();

			foreach (Team team in this.Teams)
			{
				JObject channelListResponse = SlackApiClient.GetFromSlackAPI<JObject>(team.Token,
																					  "channels.list");

				if (channelListResponse != null && channelListResponse["channels"] != null)
				{
					foreach (JToken responsedChannel in channelListResponse["channels"])
					{
						if (responsedChannel["is_member"].Value<bool>() == false)
							continue;

						team.Channels.Add(new Channel()
						{
							ID = responsedChannel["id"].Value<string>(),
							Name = responsedChannel["name"].Value<string>(),
							LatestTimestamp = team.LatestTimestamp
						});
					}
				}

				JObject groupListResponse = SlackApiClient.GetFromSlackAPI<JObject>(team.Token,
																					  "groups.list");

				if (groupListResponse != null && groupListResponse["groups"] != null)
				{
					foreach (JToken responsedGroup in groupListResponse["groups"])
					{
						if (responsedGroup["is_archived"].Value<bool>() == true)
							continue;

						team.Groups.Add(new Group()
						{
							ID = responsedGroup["id"].Value<string>(),
							Name = responsedGroup["name"].Value<string>(),
							LatestTimestamp = team.LatestTimestamp
						});
					}
				}

				JObject chatListResponse = SlackApiClient.GetFromSlackAPI<JObject>(team.Token,
																					  "im.list");

				if (chatListResponse != null && chatListResponse["ims"] != null)
				{
					foreach (JToken responsedChat in chatListResponse["ims"])
					{
						team.Chats.Add(new Chat()
						{
							ID = responsedChat["id"].Value<string>(),
							Name = responsedChat["user"].Value<string>(),
							LatestTimestamp = team.LatestTimestamp
						});
					}
				}
			}

			if (this.SyncTimer != null)
			{
				this.SyncTimer.Stop();
				this.SyncTimer = null;
			}

			this.SyncTimer = new Timer(Settings.Default.CheckInterval * 1000);
			this.SyncTimer.Elapsed += (sender, args) =>
			{
				try
				{
					foreach (Team team in this.Teams)
					{
						foreach (Channel channel in team.Channels)
						{
							JObject channelResponse = SlackApiClient.GetFromSlackAPI<JObject>(team.Token,
																							   "channels.info",
																							   string.Format("channel={0}", channel.ID));

							if (channelResponse != null &&
								channelResponse["channel"] != null)
							{
								if (channelResponse["channel"]["unread_count_visible"] != null)
									channel.HasUnread = channelResponse["channel"]["unread_count_visible"].Value<int>() > 0;
								else if (channelResponse["channel"]["unread_count"] != null)
									channel.HasUnread = channelResponse["channel"]["unread_count"].Value<int>() > 0;
								else
									channel.HasUnread = false;
							}
						}

						foreach (Group group in team.Groups)
						{
							JObject groupResponse = SlackApiClient.GetFromSlackAPI<JObject>(team.Token,
																							   "groups.history",
																							   string.Format("channel={0}", group.ID));


							if (groupResponse != null &&
								groupResponse["messages"] != null)
							{
								string latest = groupResponse["messages"].Max(r => r["ts"].Value<string>());

								if (group.HasUnread == false &&
									group.LatestTimestamp != latest)
								{
									group.LatestTimestamp = latest;
									group.HasUnread = true;
								}
							}
						}

						foreach (Chat chat in team.Chats)
						{
							JObject chatResponse = SlackApiClient.GetFromSlackAPI<JObject>(team.Token,
																							   "im.history",
																							   string.Format("channel={0}", chat.ID));


							if (chatResponse != null &&
								chatResponse["messages"] != null)
							{
								string latest = chatResponse["messages"].Max(r => r["ts"].Value<string>());

								if (chat.HasUnread == false &&
									chat.LatestTimestamp != latest)
								{
									chat.LatestTimestamp = latest;
									chat.HasUnread = true;
								}
							}
						}


					}
					Dispatcher.BeginInvoke(new Action(() =>
					{
						string message = string.Empty;

						foreach (Team team in this.Teams)
						{
							if (team.Channels.Where(c => c.HasUnread).Any())
							{
								message += string.Format("{0} 以下頻道中有新訊息喔:\n", team.Name);

								foreach (Channel channel in team.Channels.Where(c => c.HasUnread))
									message += channel.Name + "\n";
							}

							if (team.Groups.Where(c => c.HasUnread).Any())
							{
								message += string.Format("{0} 以下群組中有新訊息喔:\n", team.Name);

								foreach (Group group in team.Groups.Where(c => c.HasUnread))
									message += group.Name + "\n";
							}

							if (team.Chats.Where(c => c.HasUnread).Any())
							{
								message += string.Format("{0} 以下對話中有新訊息喔:\n", team.Name);

								foreach (Chat chat in team.Chats.Where(c => c.HasUnread))
									message += chat.Name + "\n";
							}
						}

						if (message.Length > 0)
							this.NotifyIcon.ShowBalloonTip("Slacker", message, BalloonIcon.Info);
					}));
				}
				catch (Exception ex)
				{
				}
			};

			this.SyncTimer.Start();
		}

		private void InitialNotifyIcon()
		{
			this.NotifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

			MenuItem rootTeamMenuItem = this.NotifyIcon.ContextMenu.Items[0] as MenuItem;

			rootTeamMenuItem.ItemsSource = this.Teams;
		}

		private void InitialCommands()
		{
			// Exist
			Commands.Exit.ExecuteAction = new Action<object>((parameter) =>
			{
				this.Shutdown();
			});

			// Edit Setting
			Commands.EditSetting.ExecuteAction = new Action<object>((parameter) =>
			{
				SettingWindow window = new SettingWindow()
				{
					DataContext = Settings.Default
				};

				window.ShowDialog();
			});

			// Save Setting
			Commands.SaveSetting.ExecuteAction = new Action<object>((parameter) =>
			{
				if (this.SyncTimer != null)
					this.SyncTimer.Stop();

				Settings.Default.Save();

				var settingWindows = this.Windows.Cast<SettingWindow>();

				for (int i = settingWindows.Count(); i > 0; i--)
					settingWindows.ElementAt(i - 1).Close();

				this.InitialTeams();
				//this.InitialNotifyIcon();
				this.InitialSync();
			});

			// Open Team
			Commands.OpenTeam.ExecuteAction = new Action<object>((parameter) =>
			{
				string teamName = parameter as string;
				Process.Start(string.Format("http://{0}.slack.com/", teamName));

				Team team = this.Teams.Single(t => t.Name == teamName);

				team.MarkRead();

				Settings.Default.Teams[team.Name].Latest = team.LatestTimestamp;
				Settings.Default.Save();
			});

		}

		#endregion

	}
}
