using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;

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

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			NotifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
		}
	}
}
