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
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slacker.Models;

namespace Slacker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : System.Windows.Application
	{

		/// <summary>
		/// Gets the notify icon on Windows bar.
		/// </summary>
		internal TaskbarIcon NotifyIcon
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the teams.
		/// </summary>
		internal ObservableCollection<Team> Teams
		{
			get;
			private set;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.NotifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
			this.Teams = new ObservableCollection<Team>();

			this.Teams.Add(new Team() { Token = "xoxp-2421999892-2421999894-2719715292-6d0a71" });

			Team team = this.Teams[0];

			JObject channelListResponse = GetFromSlackAPI<JObject>(team,
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

			Timer timer = new Timer(10000);
			timer.Elapsed += (sender, args) =>
			{
				foreach (Channel channel in team.Channels)
				{
					JObject channelResponse = GetFromSlackAPI<JObject>(team,
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
					if (team.Channels.Where(c => c.HasUnread).Any() &&
						App.Current.MainWindow.IsFocused == false)
					{
						string message = "以下頻道中有新訊息喔:";

						foreach (Channel channel in team.Channels.Where(c => c.HasUnread))
							message += "\n" + channel.Name;

						this.NotifyIcon.ShowBalloonTip("Slacker", message, BalloonIcon.Info);
					}
				}));

			};

			timer.Start();

		}


		private T GetFromSlackAPI<T>(Team team,
									 string urlWithoutBaseUrl,
									 params string[] parameters)
		{
			T result = default(T);

			try
			{
				HttpClient client = new HttpClient();
				client.BaseAddress = new Uri("https://slack.com/api/");
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				string url = urlWithoutBaseUrl + string.Format("?token={0}", team.Token);

				foreach (string parameter in parameters)
					url += "&" + parameter;

				string rawResponseContent = client.GetAsync(url,
															HttpCompletionOption.ResponseContentRead)
												  .Result
												  .Content
												  .ReadAsStringAsync()
												  .Result;

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				result = JsonConvert.DeserializeObject<T>(rawResponseContent);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message + ex.StackTrace);
			}

			return result;
		}
	}
}
