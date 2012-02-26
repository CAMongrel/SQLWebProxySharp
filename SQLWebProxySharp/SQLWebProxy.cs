using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using MySql.Data.MySqlClient;

namespace SQLWebProxySharp
{
	public class SQLWebProxy
	{
		private Webserver webserver;

		public event LogOutputDelegate OnLogOutput;

		public SQLWebProxy(string listenServer = "localhost", int listenPort = 8080)
		{
			webserver = new Webserver();
			webserver.OnLogOutput += (line) =>
				{
					if (OnLogOutput != null)
						OnLogOutput(line);
				};
			webserver.OnReceiveRequest += webserver_OnReceiveRequest;
			webserver.AddPrefix("http://" + listenServer + ":" + listenPort + "/");
		}

		private string ConstructError(string errorMessage)
		{
			return "<SQLWebProxyResult><Error>" + errorMessage + "</Error></SQLWebProxyResult>";
		}

		private string ConstructOk()
		{
			return "<SQLWebProxyResult><Ok /></SQLWebProxyResult>";
		}

		private string ConstructReader(object[][] rows)
		{
			int rowCount = rows.Length;
			int fieldCount = 0;
			if (rows.Length > 0)
				fieldCount = rows[0].Length;

			string result = "<SQLWebProxyResult>";
			result += "<Reader RowCount=\"" + rowCount + "\" FieldCount=\"" + fieldCount + "\">";
			for (int i = 0; i < rows.Length; i++)
			{
				result += "<Row>";
				for (int j = 0; j < rows[i].Length; j++)
				{
					object obj = rows[i][j];
					string typeName = "null";
					if (obj != null)
						typeName = obj.GetType().Name;

					result += "<Column Type=\"" + typeName + "\">" + obj + "</Column>";
				}
				result += "</Row>";
			}
			result += "</Reader>";
			result += "<SQLWebProxyResult>";

			return result;
		}

		private string ConstructScalar(object obj)
		{
			string typeName = "null";
			if (obj != null)
				typeName = obj.GetType().Name;

			return "<SQLWebProxyResult><Scalar Type=\"" + typeName + "\">" + obj + "</Scalar></SQLWebProxyResult>";
		}

		private string ConstructNonQuery(int result)
		{
			return "<SQLWebProxyResult><NonQuery>" + result.ToString() + "</NonQuery></SQLWebProxyResult>";
		}

		private string webserver_OnReceiveRequest(System.Net.HttpListenerRequest request)
		{
			Uri uri = request.Url;
			NameValueCollection parameters = HttpUtility.ParseQueryString(uri.Query);
			string mode = parameters["mode"];

			switch (mode)
			{
				case null:
					return ConstructError("NO_MODE");

				case "open":
					{
						if (DBConnect.State != System.Data.ConnectionState.Closed)
							return ConstructError("ALREADY_CONNECTED");

						string connString = parameters["connString"];
						try
						{
							DBConnect.Connect(connString);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}

						if (DBConnect.State == System.Data.ConnectionState.Open)
							return ConstructOk();

						return ConstructError("CONNECT_FAILED");
					}

				case "close":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						try
						{
							DBConnect.Close();
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}

						if (DBConnect.State == System.Data.ConnectionState.Closed)
							return ConstructOk();

						return ConstructError("DISCONNECT_FAILED");
					}

				case "reader":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						string query = parameters["query"];

						try
						{
							MySqlDataReader reader = DBConnect.ExecuteReader(query);

							List<object[]> resultArray = new List<object[]>();
							while (reader.Read())
							{
								object[] row = new object[reader.VisibleFieldCount];
								for (int i = 0; i < reader.VisibleFieldCount; i++)
									row[i] = reader.GetValue(i);
								resultArray.Add(row);
							}

							return ConstructReader(resultArray.ToArray());
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}
					break;

				case "scalar":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						string query = parameters["query"];

						try
						{
							object result = DBConnect.ExecuteScalar(query);
							return ConstructScalar(result);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}

				case "nonquery":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						string query = parameters["query"];

						try
						{
							int result = DBConnect.ExecuteNonQuery(query);
							return ConstructNonQuery(result);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}
			}

			// Empty response
			string responseString = "<SQLWebProxyResult></SQLWebProxyResult>";
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