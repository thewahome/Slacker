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

			foreach (TeamConfigElement teamConfig in Globals.Settings.Teams)
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
							Name = responsedChannel["name"].Value<string>()
						});
					}
				}
			}

			if (this.SyncTimer != null)
			{
				this.SyncTimer.Stop();
				this.SyncTimer = null;
			}

			this.SyncTimer = new Timer(Globals.Settings.CheckInterval * 1000);
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

						Dispatcher.BeginInvoke(new Action(() =>
						{
							if (team.Channels.Where(c => c.HasUnread).Any())
							{
								string message = string.Format("{0} 以下頻道中有新訊息喔:", team.Name);

								foreach (Channel channel in team.Channels.Where(c => c.HasUnread))
									message += "\n" + channel.Name;

								this.NotifyIcon.ShowBalloonTip("Slacker", message, BalloonIcon.Info);
							}
						}));

					}
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
			//rootTeamMenuItem.Items.Clear();

			//foreach (Team team in this.Teams)
			//{
			//	rootTeamMenuItem.Items.Add(new MenuItem() { Header = team.Name });
			//}
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
					DataContext = Globals.Settings
				};

				window.ShowDialog();
			});

			// Save Setting
			Commands.SaveSetting.ExecuteAction = new Action<object>((parameter) =>
			{
				if (this.SyncTimer != null)
					this.SyncTimer.Stop();

				Globals.Settings.Save();

				var settingWindows = this.Windows.Cast<SettingWindow>();

				for (int i = settingWindows.Count(); i > 0; i--)
					settingWindows.ElementAt(i-1).Close();

				this.InitialTeams();
				//this.InitialNotifyIcon();
				this.InitialSync();
			});

			// Open Team
			Commands.OpenTeam.ExecuteAction = new Action<object>((parameter) =>
			{ 
				string teamName = parameter as string;
				Process.Start(string.Format("http://{0}.slack.com/", teamName));
			});

		}

		#endregion

	}
}
