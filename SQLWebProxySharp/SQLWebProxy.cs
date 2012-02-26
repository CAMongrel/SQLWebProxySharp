using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace MongrelWebServer
{
	public class SQLWebProxy
	{
		private Webserver webserver;

		public event LogOutputDelegate OnLogOutput;

		public SQLWebProxy()
		{
			webserver = new Webserver();
			webserver.OnLogOutput += (line) =>
				{
					if (OnLogOutput != null)
						OnLogOutput(line);
				};
			webserver.OnReceiveRequest += webserver_OnReceiveRequest;
			webserver.AddPrefix("http://localhost:8080/");
		}

		string webserver_OnReceiveRequest(System.Net.HttpListenerRequest request)
		{
			Uri uri = request.Url;
			NameValueCollection parameters = HttpUtility.ParseQueryString(uri.Query);
			try
			{
				string query = parameters["query"];
			}
			catch (Exception ex)
			{

			}

			string responseString = "<xml></xml>";

			return responseString;
		}

		public void Start()
		{
			webserver.Start();
		}

		public void Stop()
		{
			webserver.Stop();
		}
	}
}