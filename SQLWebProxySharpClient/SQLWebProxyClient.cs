using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SQLWebProxySharpClient
{
	/// <summary>
	/// Stateless client class
	/// </summary>
	public class SQLWebProxyClient
	{
		public string RemoteAddress { get; set; }
		public int RemotePort { get; set; }

		private string RemoteServer
		{
			get
			{
				return "http://" + RemoteAddress + ":" + RemotePort + "/";
			}
		}

		public SQLWebProxyClient()
		{
			RemoteAddress = null;
			RemotePort = -1;
		}

		private void CheckSettings()
		{
			if (string.IsNullOrWhiteSpace(RemoteAddress))
				throw new Exception("RemoteAddress not set (must be a valid server address, e.g. localhost or 192.168.1.1)");
			if (RemotePort == -1 || RemotePort == 0)
				throw new Exception("RemotePort not set (must be a valid server port, e.g. 8080)");
		}

		private string GetResponse(string uriQuery)
		{
			HttpWebRequest request = HttpWebRequest.Create(uriQuery) as HttpWebRequest;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

		public void Open(string serverAdress, string userName, string password, string database, int port = 3306)
		{
			CheckSettings();

			string connString = string.Format("server={0};user={1};database={2};port={4};password={3};",
					serverAdress, userName, database, password, port.ToString());

			string response = GetResponse(RemoteServer + "?mode=open&connString=" + connString);
		}

		public void Close()
		{
			CheckSettings();

			string response = GetResponse(RemoteServer + "?mode=close");
		}

		public void ExecuteReader(string query)
		{
			CheckSettings();

			string response = GetResponse(RemoteServer + "?mode=reader&query=" + query);
		}

		public void ExecuteScalar(string query)
		{
			CheckSettings();

			string response = GetResponse(RemoteServer + "?mode=scalar&query=" + query);
		}

		public void ExecuteNonQuery(string query)
		{
			CheckSettings();

			string response = GetResponse(RemoteServer + "?mode=nonquery&query=" + query);
		}
	}
}
