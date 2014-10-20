using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Slacker.Core
{
	class SlackApiClient
	{
		private static HttpClient _client = new HttpClient();

		static SlackApiClient()
		{
			_client.BaseAddress = new Uri("https://slack.com/api/");
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public static T GetFromSlackAPI<T>(string token,
										   string urlWithoutBaseUrl,
										   params string[] parameters)
		{
			T result = default(T);

			try
			{

				string url = urlWithoutBaseUrl + string.Format("?token={0}", token);

				foreach (string parameter in parameters)
					url += "&" + parameter;

				string rawResponseContent = _client.GetAsync(url,
															HttpCompletionOption.ResponseContentRead)
												  .Result
												  .Content
												  .ReadAsStringAsync()
												  .Result;

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				result = JsonConvert.DeserializeObject<T>(rawResponseContent);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message + ex.StackTrace);
			}

			return result;
		}
	}
}
