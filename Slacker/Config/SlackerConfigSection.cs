using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Slacker.Config
{
	class SlackerConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("teams",
							   IsRequired = false,
							   IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(TeamConfigElementCollection),
								 AddItemName = "add")]
		public TeamConfigElementCollection Teams
		{
			get { return (TeamConfigElementCollection)base["teams"]; }
			set { base["teams"] = value; }
		}

		/// <summary>
		/// Get or Set Check Interval(unit: minutes)
		/// </summary>
		[ConfigurationProperty("checkInterval",
							   IsRequired = false,
							   DefaultValue = 3)]
		public int CheckInterval
		{
			get { return (int)base["checkInterval"]; }
			set { base["checkInterval"] = value; }
		}

		public void Save()
		{
			int i = this.Teams.Count;

			while (i > 0)
			{
				if (string.IsNullOrEmpty(this.Teams[i - 1].Name))
					this.Teams.Remove(this.Teams[i - 1]);
				i--;
			}

			if (this.CurrentConfiguration == null)
			{
				Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

				if (config.Sections["slacker"] == null)
					config.Sections.Add("slacker", this);

				config.Save();
			}
			else
			{
				if(this.CurrentConfiguration.Sections["slacker"] == null)
					this.CurrentConfiguration.Sections.Add("slacker", this);

				this.CurrentConfiguration.Save();
			}
		}
	}
}
